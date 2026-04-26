using FluentValidation;
using MediatR;
using PsychoSupCenterBackend.Application.Common.Behaviors;
using PsychoSupCenterBackend.Application.Common.Interfaces;
using PsychoSupCenterBackend.Application.Common.Models;
using PsychoSupCenterBackend.Application.Reviews.DTOs;
using PsychoSupCenterBackend.Domain.Entities;

namespace PsychoSupCenterBackend.Application.Reviews.Commands;

public static class CreateReview
{
    public sealed record Command(CreateReviewDto Dto) : ICommand<Result<ReviewResponseDto>>;

    public sealed class Validator : AbstractValidator<Command>
    {
        public Validator()
        {
            RuleFor(x => x.Dto.Rating).InclusiveBetween(1, 5).WithMessage("Рейтинг має бути від 1 до 5.");
            RuleFor(x => x.Dto.Comment).MaximumLength(1000);
            RuleFor(x => x.Dto.AppointmentId).NotEmpty();
        }
    }

    public sealed class Handler(IUnitOfWork unitOfWork) : IRequestHandler<Command, Result<ReviewResponseDto>>
    {
        public async Task<Result<ReviewResponseDto>> Handle(Command request, CancellationToken cancellationToken)
        {
            var existingReview = await unitOfWork.Reviews.AnyAsync(r => r.AppointmentId == request.Dto.AppointmentId, cancellationToken);
            if (existingReview) return Result<ReviewResponseDto>.Failure("Відгук для цього запису вже існує.");

            var review = new Review
            {
                Id = Guid.NewGuid(),
                DoctorProfileId = request.Dto.DoctorProfileId,
                PatientProfileId = request.Dto.PatientProfileId,
                AppointmentId = request.Dto.AppointmentId,
                Rating = request.Dto.Rating,
                Comment = request.Dto.Comment,
                IsAnonymous = request.Dto.IsAnonymous,
                CreatedAt = DateTime.UtcNow
            };

            await unitOfWork.Reviews.AddAsync(review, cancellationToken);

            var doctor = await unitOfWork.DoctorProfiles.GetByIdAsync(request.Dto.DoctorProfileId, cancellationToken);
            if (doctor is not null)
            {
                var allDoctorReviews = await unitOfWork.Reviews.FindAsync(r => r.DoctorProfileId == doctor.Id, cancellationToken);
                var totalRating = allDoctorReviews.Sum(r => r.Rating) + review.Rating;
                doctor.AverageRating = totalRating / (allDoctorReviews.Count + 1.0);
                unitOfWork.DoctorProfiles.Update(doctor);
            }

            return Result<ReviewResponseDto>.Success(new ReviewResponseDto(
                review.Id, review.DoctorProfileId, review.PatientProfileId, review.AppointmentId,
                review.Rating, review.Comment, review.IsAnonymous, review.CreatedAt));
        }
    }
}