
using FluentValidation;
using MediatR;
using PsychoSupCenterBackend.Application.Common.Behaviors;
using PsychoSupCenterBackend.Application.Common.Interfaces;
using PsychoSupCenterBackend.Application.Common.Models;
using PsychoSupCenterBackend.Application.DoctorAvailabilities.DTOs;

namespace PsychoSupCenterBackend.Application.DoctorAvailabilities.Queries;

public static class GetDoctorAvailabilityById
{
    public sealed record Query(Guid AvailabilityId)
        : IQuery<Result<DoctorAvailabilityResponseDto>>;

    public sealed class Validator : AbstractValidator<Query>
    {
        public Validator() => RuleFor(x => x.AvailabilityId).NotEmpty();
    }

    public sealed class Handler(IUnitOfWork unitOfWork)
        : IRequestHandler<Query, Result<DoctorAvailabilityResponseDto>>
    {
        public async Task<Result<DoctorAvailabilityResponseDto>> Handle(
            Query request, CancellationToken cancellationToken)
        {
            var a = await unitOfWork.DoctorAvailabilities
                .GetByIdAsync(request.AvailabilityId, cancellationToken);

            if (a is null)
                return Result<DoctorAvailabilityResponseDto>.Failure("Слот не знайдено.");

            return Result<DoctorAvailabilityResponseDto>.Success(
                new DoctorAvailabilityResponseDto(
                    a.Id, a.DoctorProfileId, a.Day, a.StartTime, a.EndTime));
        }
    }
}