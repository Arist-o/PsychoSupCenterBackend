using FluentValidation;
using MediatR;
using PsychoSupCenterBackend.Application.Common.Behaviors;
using PsychoSupCenterBackend.Application.Common.Interfaces;
using PsychoSupCenterBackend.Application.Common.Models;
using PsychoSupCenterBackend.Application.Appointments.DTOs;

namespace PsychoSupCenterBackend.Application.Appointments.Commands;

public static class UpdateAppointment
{
    public sealed record Command(Guid AppointmentId, UpdateAppointmentDto Dto) : ICommand<Result<AppointmentResponseDto>>;

    public sealed class Validator : AbstractValidator<Command>
    {
        public Validator()
        {
            RuleFor(x => x.AppointmentId).NotEmpty();
            RuleFor(x => x.Dto.DurationMinutes).GreaterThan(0);
            RuleFor(x => x.Dto.Status).IsInEnum();
        }
    }

    public sealed class Handler(IUnitOfWork unitOfWork) : IRequestHandler<Command, Result<AppointmentResponseDto>>
    {
        public async Task<Result<AppointmentResponseDto>> Handle(Command request, CancellationToken cancellationToken)
        {
            var appointment = await unitOfWork.Appointments.GetByIdAsync(request.AppointmentId, cancellationToken);
            if (appointment is null) return Result<AppointmentResponseDto>.Failure("Запис не знайдено.");

            appointment.ScheduledAt = request.Dto.ScheduledAt;
            appointment.DurationMinutes = request.Dto.DurationMinutes;
            appointment.Status = request.Dto.Status;
            appointment.Type = request.Dto.Type;
            appointment.Notes = request.Dto.Notes;

            unitOfWork.Appointments.Update(appointment);

            return Result<AppointmentResponseDto>.Success(new AppointmentResponseDto(
                appointment.Id, appointment.DoctorProfileId, appointment.PatientProfileId, appointment.DoctorServiceId,
                appointment.ChatRoomId, appointment.BillingId, appointment.ScheduledAt, appointment.DurationMinutes,
                appointment.Status, appointment.Type, appointment.Notes, appointment.CreatedAt));
        }
    }
}