using FluentValidation;
using MediatR;
using PsychoSupCenterBackend.Application.Common.Behaviors;
using PsychoSupCenterBackend.Application.Common.Interfaces;
using PsychoSupCenterBackend.Application.Common.Models;
using PsychoSupCenterBackend.Application.DoctorAvailabilities.DTOs;

namespace PsychoSupCenterBackend.Application.DoctorAvailabilities.Queries;

public static class GetAvailabilitiesByDoctorId
{
    public sealed record Query(Guid DoctorProfileId) : IQuery<Result<IReadOnlyList<DoctorAvailabilityResponseDto>>>;

    public sealed class Validator : AbstractValidator<Query>
    {
        public Validator() => RuleFor(x => x.DoctorProfileId).NotEmpty();
    }

    public sealed class Handler(IUnitOfWork unitOfWork) : IRequestHandler<Query, Result<IReadOnlyList<DoctorAvailabilityResponseDto>>>
    {
        public async Task<Result<IReadOnlyList<DoctorAvailabilityResponseDto>>> Handle(Query request, CancellationToken cancellationToken)
        {
            var availabilities = await unitOfWork.DoctorAvailabilities
                .FindAsync(a => a.DoctorProfileId == request.DoctorProfileId, cancellationToken);

            var dtos = availabilities.Select(a =>
                new DoctorAvailabilityResponseDto(a.Id, a.DoctorProfileId, a.Day, a.StartTime, a.EndTime, true))
                .ToList();

            return Result<IReadOnlyList<DoctorAvailabilityResponseDto>>.Success(dtos);
        }
    }
}