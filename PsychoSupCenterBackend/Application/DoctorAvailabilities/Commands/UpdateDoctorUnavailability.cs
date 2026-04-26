using Application.Doctors.DTOs;
using FluentValidation;
using MediatR;
using PsychoSupCenterBackend.Application.Common.Behaviors;
using PsychoSupCenterBackend.Application.Common.Interfaces;
using PsychoSupCenterBackend.Application.Common.Models;
using PsychoSupCenterBackend.Application.DoctorAvailabilities.DTOs;

namespace PsychoSupCenterBackend.Application.DoctorAvailabilities.Commands;

public static class UpdateDoctorUnavailability
{
    public sealed record Command(
        Guid UnavailabilityId,
        UpdateUnavailabilityBodyDto Dto
    ) : ICommand<Result<DoctorUnavailabilityResponseDto>>;

    public sealed class Validator : AbstractValidator<Command>
    {
        public Validator()
        {
            RuleFor(x => x.UnavailabilityId).NotEmpty();
            RuleFor(x => x.Dto.StartDate).LessThan(x => x.Dto.EndDate).WithMessage("Дата початку має бути раніше дати кінця.");
            RuleFor(x => x.Dto.Reason).MaximumLength(500).When(x => x.Dto.Reason is not null);
        }
    }

    public sealed class Handler(IUnitOfWork unitOfWork) : IRequestHandler<Command, Result<DoctorUnavailabilityResponseDto>>
    {
        public async Task<Result<DoctorUnavailabilityResponseDto>> Handle(Command request, CancellationToken cancellationToken)
        {
            var unavailability = await unitOfWork.DoctorUnavailabilities.GetByIdAsync(request.UnavailabilityId, cancellationToken);
            if (unavailability is null) return Result<DoctorUnavailabilityResponseDto>.Failure("Запис про відсутність не знайдено.");

            unavailability.StartDate = request.Dto.StartDate;
            unavailability.EndDate = request.Dto.EndDate;
            unavailability.Reason = request.Dto.Reason;

            unitOfWork.DoctorUnavailabilities.Update(unavailability);

            return Result<DoctorUnavailabilityResponseDto>.Success(
                new DoctorUnavailabilityResponseDto(unavailability.Id, unavailability.DoctorProfileId, unavailability.StartDate, unavailability.EndDate, unavailability.Reason));
        }
    }
}