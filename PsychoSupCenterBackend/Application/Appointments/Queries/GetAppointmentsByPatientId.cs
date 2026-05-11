using FluentValidation;
using MediatR;
using Microsoft.EntityFrameworkCore;
using PsychoSupCenterBackend.Application.Common.Behaviors;
using PsychoSupCenterBackend.Application.Common.Interfaces;
using PsychoSupCenterBackend.Application.Common.Models;
using PsychoSupCenterBackend.Application.Appointments.DTOs;

namespace PsychoSupCenterBackend.Application.Appointments.Queries;

public static class GetAppointmentsByPatientId
{
    public sealed record Query(Guid PatientProfileId, int Page = 1, int PageSize = 20) : IQuery<Result<IReadOnlyList<AppointmentResponseDto>>>;

    public sealed class Validator : AbstractValidator<Query>
    {
        public Validator() => RuleFor(x => x.PatientProfileId).NotEmpty();
    }

    public sealed class Handler(IUnitOfWork unitOfWork) : IRequestHandler<Query, Result<IReadOnlyList<AppointmentResponseDto>>>
    {
        public async Task<Result<IReadOnlyList<AppointmentResponseDto>>> Handle(Query request, CancellationToken cancellationToken)
        {
            var appointments = await unitOfWork.Appointments.Query()
                .Include(a => a.DoctorProfile).ThenInclude(dp => dp.User)
                .Include(a => a.DoctorProfile).ThenInclude(dp => dp.Specializations)
                .Include(a => a.PatientProfile).ThenInclude(pp => pp.User)
                .Include(a => a.DoctorService)
                .Where(a => a.PatientProfileId == request.PatientProfileId)
                .OrderByDescending(a => a.ScheduledAt)
                .Skip((request.Page - 1) * request.PageSize)
                .Take(request.PageSize)
                .ToListAsync(cancellationToken);

            var result = appointments
                .Select(appt => new AppointmentResponseDto(
                    appt.Id, appt.DoctorProfileId, appt.PatientProfileId, appt.DoctorServiceId,
                    appt.ChatRoomId, appt.BillingId, appt.ScheduledAt, appt.DurationMinutes,
                    appt.Status, appt.Type ?? "Consultation", appt.Notes, appt.CreatedAt,
                    $"{appt.DoctorProfile?.User?.FirstName} {appt.DoctorProfile?.User?.LastName}".Trim(),
                    appt.DoctorProfile?.User?.PhotoUrl,
                    appt.DoctorProfile?.Specializations?.FirstOrDefault()?.Name,
                    appt.DoctorService?.ServiceName,
                    $"{appt.PatientProfile?.User?.FirstName} {appt.PatientProfile?.User?.LastName}".Trim(),
                    appt.PatientProfile?.User?.PhotoUrl
                )).ToList();

            return Result<IReadOnlyList<AppointmentResponseDto>>.Success(result);
        }
    }
}