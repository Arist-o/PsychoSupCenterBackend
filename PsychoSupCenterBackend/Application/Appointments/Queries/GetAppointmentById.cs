using FluentValidation;
using MediatR;
using Microsoft.EntityFrameworkCore;
using PsychoSupCenterBackend.Application.Common.Behaviors;
using PsychoSupCenterBackend.Application.Common.Interfaces;
using PsychoSupCenterBackend.Application.Common.Models;
using PsychoSupCenterBackend.Application.Appointments.DTOs;

namespace PsychoSupCenterBackend.Application.Appointments.Queries;

public static class GetAppointmentById
{
    public sealed record Query(Guid AppointmentId) : IQuery<Result<AppointmentResponseDto>>;

    public sealed class Validator : AbstractValidator<Query>
    {
        public Validator() => RuleFor(x => x.AppointmentId).NotEmpty();
    }

    public sealed class Handler(IUnitOfWork unitOfWork) : IRequestHandler<Query, Result<AppointmentResponseDto>>
    {
        public async Task<Result<AppointmentResponseDto>> Handle(Query request, CancellationToken cancellationToken)
        {
            var appt = await unitOfWork.Appointments.Query()
                .Include(a => a.DoctorProfile).ThenInclude(dp => dp.User)
                .Include(a => a.DoctorProfile).ThenInclude(dp => dp.Specializations)
                .Include(a => a.PatientProfile).ThenInclude(pp => pp.User)
                .Include(a => a.DoctorService)
                .FirstOrDefaultAsync(a => a.Id == request.AppointmentId, cancellationToken);
                
            if (appt is null) return Result<AppointmentResponseDto>.Failure("Запис не знайдено.");

            return Result<AppointmentResponseDto>.Success(new AppointmentResponseDto(
                appt.Id, appt.DoctorProfileId, appt.PatientProfileId, appt.DoctorServiceId,
                appt.ChatRoomId, appt.BillingId, appt.ScheduledAt, appt.DurationMinutes,
                appt.Status, appt.Type ?? "Consultation", appt.Notes, appt.CreatedAt,
                $"{appt.DoctorProfile?.User?.FirstName} {appt.DoctorProfile?.User?.LastName}".Trim(),
                appt.DoctorProfile?.User?.PhotoUrl,
                appt.DoctorProfile?.Specializations?.FirstOrDefault()?.Name,
                appt.DoctorService?.ServiceName,
                $"{appt.PatientProfile?.User?.FirstName} {appt.PatientProfile?.User?.LastName}".Trim(),
                appt.PatientProfile?.User?.PhotoUrl
            ));
        }
    }
}