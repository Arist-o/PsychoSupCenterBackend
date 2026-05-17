using MediatR;
using Microsoft.EntityFrameworkCore;
using PsychoSupCenterBackend.Application.Appointments.DTOs;
using PsychoSupCenterBackend.Application.Common.Behaviors;
using PsychoSupCenterBackend.Application.Common.Interfaces;
using PsychoSupCenterBackend.Application.Common.Models;

namespace PsychoSupCenterBackend.Application.Appointments.Queries;

public static class GetAllAppointments
{
    public sealed record Query(int Page = 1, int PageSize = 100) : IQuery<Result<IReadOnlyList<AppointmentResponseDto>>>;

    public sealed class Handler(IUnitOfWork unitOfWork) : IRequestHandler<Query, Result<IReadOnlyList<AppointmentResponseDto>>>
    {
        public async Task<Result<IReadOnlyList<AppointmentResponseDto>>> Handle(Query request, CancellationToken cancellationToken)
        {
            var appointments = await unitOfWork.Appointments.Query()
                .Include(a => a.DoctorProfile).ThenInclude(d => d.User)
                .Include(a => a.PatientProfile).ThenInclude(p => p.User)
                .Include(a => a.DoctorService)
                .OrderByDescending(a => a.ScheduledAt)
                .Skip((request.Page - 1) * request.PageSize)
                .Take(request.PageSize)
                .Select(a => new AppointmentResponseDto(
                    a.Id, a.DoctorProfileId, a.PatientProfileId, a.DoctorServiceId,
                    a.ChatRoomId, a.BillingId, a.ScheduledAt, a.DurationMinutes, a.Status,
                    a.Type, a.Notes, a.CreatedAt,
                    $"{a.DoctorProfile.User.FirstName} {a.DoctorProfile.User.LastName}",
                    a.DoctorProfile.User.PhotoUrl,
                    "", // Specialization placeholder if needed
                    a.DoctorService.ServiceName,
                    $"{a.PatientProfile.User.FirstName} {a.PatientProfile.User.LastName}",
                    a.PatientProfile.User.PhotoUrl
                ))
                .ToListAsync(cancellationToken);

            return Result<IReadOnlyList<AppointmentResponseDto>>.Success(appointments);
        }
    }
}