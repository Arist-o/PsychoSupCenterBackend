using FluentValidation;
using MediatR;
using PsychoSupCenterBackend.Application.Common.Behaviors;
using PsychoSupCenterBackend.Application.Common.Interfaces;
using PsychoSupCenterBackend.Application.Common.Models;
using PsychoSupCenterBackend.Application.Reviews.DTOs;

namespace PsychoSupCenterBackend.Application.Reviews.Queries;

public static class GetReviewById
{
    public sealed record Query(Guid ReviewId) : IQuery<Result<ReviewResponseDto>>;

    public sealed class Validator : AbstractValidator<Query>
    {
        public Validator() => RuleFor(x => x.ReviewId).NotEmpty();
    }

    public sealed class Handler(IUnitOfWork unitOfWork) : IRequestHandler<Query, Result<ReviewResponseDto>>
    {
        public async Task<Result<ReviewResponseDto>> Handle(Query request, CancellationToken cancellationToken)
        {
            var r = await unitOfWork.Reviews.GetByIdAsync(request.ReviewId, cancellationToken);
            if (r is null) return Result<ReviewResponseDto>.Failure("Відгук не знайдено.");

            return Result<ReviewResponseDto>.Success(new ReviewResponseDto(
                r.Id, r.DoctorProfileId, r.PatientProfileId, r.AppointmentId,
                r.Rating, r.Comment, r.IsAnonymous, r.CreatedAt));
        }
    }
}