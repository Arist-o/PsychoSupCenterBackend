using PsychoSupCenterBackend.Domain.Enums;

namespace PsychoSupCenterBackend.Application.Appointments.DTOs;

public sealed record AppointmentResponseDto(
    Guid Id,
    Guid DoctorProfileId,
    Guid PatientProfileId,
    Guid DoctorServiceId,
    Guid? ChatRoomId, 
    Guid? BillingId,  
    DateTime ScheduledAt,
    int DurationMinutes,
    AppointmentStatus Status,
    string Type,
    string? Notes,
    DateTime CreatedAt
);