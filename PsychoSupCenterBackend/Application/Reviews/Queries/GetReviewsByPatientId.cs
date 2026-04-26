using FluentValidation;
using MediatR;
using PsychoSupCenterBackend.Application.Common.Behaviors;
using PsychoSupCenterBackend.Application.Common.Interfaces;
using PsychoSupCenterBackend.Application.Common.Models;
using PsychoSupCenterBackend.Application.Reviews.DTOs;

namespace PsychoSupCenterBackend.Application.Reviews.Queries;

public static class GetReviewsByPatientId
{
    public sealed record Query(Guid PatientProfileId    ) : IQuery<Result<IReadOnlyList<ReviewResponseDto>>>;

    public sealed class Validator : AbstractValidator<Query>
    {
        public Validator() => RuleFor(x => x.PatientProfileId).NotEmpty();
    }

    public sealed class Handler(IUnitOfWork unitOfWork) : IRequestHandler<Query, Result<IReadOnlyList<ReviewResponseDto>>>
    {
        public async Task<Result<IReadOnlyList<ReviewResponseDto>>> Handle(Query request, CancellationToken cancellationToken)
        {
            var reviews = await unitOfWork.Reviews.FindAsync(r => r.PatientProfileId == request.PatientProfileId, cancellationToken);

            var result = reviews.Select(r => new ReviewResponseDto(
                r.Id, r.DoctorProfileId, r.PatientProfileId, r.AppointmentId,
                r.Rating, r.Comment, r.IsAnonymous, r.CreatedAt)).ToList();

            return Result<IReadOnlyList<ReviewResponseDto>>.Success(result);
        }
    }
}