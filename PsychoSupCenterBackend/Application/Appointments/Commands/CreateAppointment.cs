using FluentValidation;
using MediatR;
using PsychoSupCenterBackend.Application.Common.Behaviors;
using PsychoSupCenterBackend.Application.Common.Interfaces;
using PsychoSupCenterBackend.Application.Common.Models;
using PsychoSupCenterBackend.Application.Appointments.DTOs;
using PsychoSupCenterBackend.Domain.Entities;
using PsychoSupCenterBackend.Domain.Enums;
using Microsoft.EntityFrameworkCore;

namespace PsychoSupCenterBackend.Application.Appointments.Commands;

public static class CreateAppointment
{
    public sealed record Command(CreateAppointmentDto Dto) : ICommand<Result<AppointmentResponseDto>>;

    public sealed class Validator : AbstractValidator<Command>
    {
        public Validator()
        {
            RuleFor(x => x.Dto.DoctorProfileId).NotEmpty();
            RuleFor(x => x.Dto.PatientProfileId).NotEmpty();
            RuleFor(x => x.Dto.DoctorServiceId).NotEmpty();
            RuleFor(x => x.Dto.ScheduledAt).GreaterThan(DateTime.UtcNow).WithMessage("Час запису має бути в майбутньому.");
            RuleFor(x => x.Dto.DurationMinutes).GreaterThan(0);
        }
    }

    public sealed class Handler(IUnitOfWork unitOfWork) : IRequestHandler<Command, Result<AppointmentResponseDto>>
    {
        public async Task<Result<AppointmentResponseDto>> Handle(Command request, CancellationToken cancellationToken)
        {
            var service = await unitOfWork.DoctorServices.GetByIdAsync(request.Dto.DoctorServiceId, cancellationToken);
            if (service is null) return Result<AppointmentResponseDto>.Failure("Обрану послугу не знайдено.");

            var doctorProfile = await unitOfWork.DoctorProfiles.Query()
                .Include(dp => dp.User)
                .Include(dp => dp.Specializations)
                .FirstOrDefaultAsync(dp => dp.Id == request.Dto.DoctorProfileId, cancellationToken);
            if (doctorProfile is null) return Result<AppointmentResponseDto>.Failure("Лікаря не знайдено.");

            var patientProfile = await unitOfWork.PatientProfiles.Query()
                .Include(pp => pp.User)
                .FirstOrDefaultAsync(pp => pp.Id == request.Dto.PatientProfileId, cancellationToken);
            if (patientProfile is null) return Result<AppointmentResponseDto>.Failure("Пацієнта не знайдено.");

            var chatRoom = new ChatRoom { Id = Guid.NewGuid(), Type = ChatType.Appointment, CreatedAt = DateTime.UtcNow };
            
            var doctorParticipant = new ChatParticipant { Id = Guid.NewGuid(), ChatRoomId = chatRoom.Id, UserId = doctorProfile.UserId };
            var patientParticipant = new ChatParticipant { Id = Guid.NewGuid(), ChatRoomId = chatRoom.Id, UserId = patientProfile.UserId };

            var billing = new Domain.Entities.Billing { Id = Guid.NewGuid(), DoctorServiceId = service.Id, Amount = service.Price, PaymentStatus = PaymentStatus.Pending, CreatedAt = DateTime.UtcNow };

            var appointment = new Appointment
            {
                Id = Guid.NewGuid(),
                DoctorProfileId = request.Dto.DoctorProfileId,
                PatientProfileId = request.Dto.PatientProfileId,
                DoctorServiceId = request.Dto.DoctorServiceId,
                ChatRoomId = chatRoom.Id,
                BillingId = billing.Id,
                ScheduledAt = request.Dto.ScheduledAt,
                DurationMinutes = request.Dto.DurationMinutes,
                Status = AppointmentStatus.Scheduled,
                Type = request.Dto.Type,
                Notes = request.Dto.Notes,
                CreatedAt = DateTime.UtcNow
            };

            await unitOfWork.ChatRooms.AddAsync(chatRoom, cancellationToken);
            await unitOfWork.ChatParticipants.AddAsync(doctorParticipant, cancellationToken);
            await unitOfWork.ChatParticipants.AddAsync(patientParticipant, cancellationToken);
            await unitOfWork.Billings.AddAsync(billing, cancellationToken);
            await unitOfWork.Appointments.AddAsync(appointment, cancellationToken);

            return Result<AppointmentResponseDto>.Success(new AppointmentResponseDto(
                appointment.Id, appointment.DoctorProfileId, appointment.PatientProfileId, appointment.DoctorServiceId,
                appointment.ChatRoomId, appointment.BillingId, appointment.ScheduledAt, appointment.DurationMinutes,
                appointment.Status, appointment.Type ?? "Consultation", appointment.Notes, appointment.CreatedAt,
                $"{doctorProfile.User?.FirstName} {doctorProfile.User?.LastName}".Trim(),
                doctorProfile.User?.PhotoUrl,
                doctorProfile.Specializations?.FirstOrDefault()?.Name,
                service.ServiceName,
                $"{patientProfile.User?.FirstName} {patientProfile.User?.LastName}".Trim(),
                patientProfile.User?.PhotoUrl
            ));
        }
    }
}