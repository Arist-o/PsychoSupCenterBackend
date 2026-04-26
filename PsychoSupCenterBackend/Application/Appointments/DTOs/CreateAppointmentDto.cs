namespace PsychoSupCenterBackend.Application.Appointments.DTOs;

public sealed record CreateAppointmentDto(
    Guid DoctorProfileId,
    Guid PatientProfileId,
    Guid DoctorServiceId,
    DateTime ScheduledAt,
    int DurationMinutes,
    string Type,
    string? Notes
);