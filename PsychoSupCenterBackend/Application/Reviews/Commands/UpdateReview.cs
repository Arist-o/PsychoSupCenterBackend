using FluentValidation;
using MediatR;
using PsychoSupCenterBackend.Application.Common.Behaviors;
using PsychoSupCenterBackend.Application.Common.Interfaces;
using PsychoSupCenterBackend.Application.Common.Models;
using PsychoSupCenterBackend.Application.Reviews.DTOs;

namespace PsychoSupCenterBackend.Application.Reviews.Commands;

public static class UpdateReview
{
    public sealed record Command(Guid ReviewId, UpdateReviewDto Dto) : ICommand<Result<ReviewResponseDto>>;

    public sealed class Validator : AbstractValidator<Command>
    {
        public Validator()
        {
            RuleFor(x => x.ReviewId).NotEmpty();
            RuleFor(x => x.Dto.Rating).InclusiveBetween(1, 5);
            RuleFor(x => x.Dto.Comment).MaximumLength(1000);
        }
    }

    public sealed class Handler(IUnitOfWork unitOfWork) : IRequestHandler<Command, Result<ReviewResponseDto>>
    {
        public async Task<Result<ReviewResponseDto>> Handle(Command request, CancellationToken cancellationToken)
        {
            var review = await unitOfWork.Reviews.GetByIdAsync(request.ReviewId, cancellationToken);
            if (review is null) return Result<ReviewResponseDto>.Failure("Відгук не знайдено.");

            review.Rating = request.Dto.Rating;
            review.Comment = request.Dto.Comment;
            review.IsAnonymous = request.Dto.IsAnonymous;

            unitOfWork.Reviews.Update(review);

            var doctor = await unitOfWork.DoctorProfiles.GetByIdAsync(review.DoctorProfileId, cancellationToken);
            if (doctor is not null)
            {
                var allReviews = await unitOfWork.Reviews.FindAsync(r => r.DoctorProfileId == doctor.Id, cancellationToken);
                doctor.AverageRating = allReviews.Any() ? allReviews.Average(r => r.Rating) : 0;
                unitOfWork.DoctorProfiles.Update(doctor);
            }

            return Result<ReviewResponseDto>.Success(new ReviewResponseDto(
                review.Id, review.DoctorProfileId, review.PatientProfileId, review.AppointmentId,
                review.Rating, review.Comment, review.IsAnonymous, review.CreatedAt));
        }
    }
}