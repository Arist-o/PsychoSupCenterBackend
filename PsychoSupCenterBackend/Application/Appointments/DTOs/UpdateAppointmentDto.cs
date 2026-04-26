using PsychoSupCenterBackend.Domain.Enums;

namespace PsychoSupCenterBackend.Application.Appointments.DTOs;

public sealed record UpdateAppointmentDto(
    DateTime ScheduledAt,
    int DurationMinutes,
    AppointmentStatus Status,
    string Type,
    string? Notes
);