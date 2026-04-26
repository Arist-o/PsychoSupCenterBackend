using Application.DoctorAvailabilities.DTOs;
using FluentValidation;
using MediatR;
using PsychoSupCenterBackend.Application.Common.Behaviors;
using PsychoSupCenterBackend.Application.Common.Interfaces;
using PsychoSupCenterBackend.Application.Common.Models;
using PsychoSupCenterBackend.Application.DoctorAvailabilities.DTOs;

namespace PsychoSupCenterBackend.Application.DoctorAvailabilities.Commands;

public static class UpdateDoctorAvailability
{
    public sealed record Command(Guid AvailabilityId, UpdateDoctorAvailabilityDto Dto) : ICommand<Result<DoctorAvailabilityResponseDto>>;

    public sealed class Validator : AbstractValidator<Command>
    {
        public Validator()
        {
            RuleFor(x => x.AvailabilityId).NotEmpty();
            RuleFor(x => x.Dto.StartTime)
                .LessThan(x => x.Dto.EndTime)
                .WithMessage("Час початку має бути раніше часу кінця.");
        }
    }

    public sealed class Handler(IUnitOfWork unitOfWork)
        : IRequestHandler<Command, Result<DoctorAvailabilityResponseDto>>
    {
        public async Task<Result<DoctorAvailabilityResponseDto>> Handle(
            Command request, CancellationToken cancellationToken)
        {
            var availability = await unitOfWork.DoctorAvailabilities
                .GetByIdAsync(request.AvailabilityId, cancellationToken);

            if (availability is null)
                return Result<DoctorAvailabilityResponseDto>.Failure("Слот не знайдено.");

            availability.Day = request.Dto.Day;
            availability.StartTime = request.Dto.StartTime;
            availability.EndTime = request.Dto.EndTime;

            unitOfWork.DoctorAvailabilities.Update(availability);

            return Result<DoctorAvailabilityResponseDto>.Success(
                new DoctorAvailabilityResponseDto(
                    availability.Id, availability.DoctorProfileId,
                    availability.Day, availability.StartTime, availability.EndTime));
        }
    }
}