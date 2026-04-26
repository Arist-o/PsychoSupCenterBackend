using FluentValidation;
using MediatR;
using PsychoSupCenterBackend.Application.Common.Behaviors;
using PsychoSupCenterBackend.Application.Common.Interfaces;
using PsychoSupCenterBackend.Application.Common.Models;

namespace PsychoSupCenterBackend.Application.Reviews.Commands;

public static class DeleteReview
{
    public sealed record Command(Guid ReviewId) : ICommand<Result<bool>>;

    public sealed class Validator : AbstractValidator<Command>
    {
        public Validator() => RuleFor(x => x.ReviewId).NotEmpty();
    }

    public sealed class Handler(IUnitOfWork unitOfWork) : IRequestHandler<Command, Result<bool>>
    {
        public async Task<Result<bool>> Handle(Command request, CancellationToken cancellationToken)
        {
            var review = await unitOfWork.Reviews.GetByIdAsync(request.ReviewId, cancellationToken);
            if (review is null) return Result<bool>.Failure("Відгук не знайдено.");

            unitOfWork.Reviews.Remove(review);

            var doctor = await unitOfWork.DoctorProfiles.GetByIdAsync(review.DoctorProfileId, cancellationToken);
            if (doctor is not null)
            {
                var remainingReviews = await unitOfWork.Reviews.FindAsync(r => r.DoctorProfileId == doctor.Id && r.Id != review.Id, cancellationToken);
                doctor.AverageRating = remainingReviews.Any() ? remainingReviews.Average(r => r.Rating) : 0;
                unitOfWork.DoctorProfiles.Update(doctor);
            }

            return Result<bool>.Success(true);
        }
    }
}