
namespace PsychoSupCenterBackend.Application.DoctorAvailabilities.DTOs;

public sealed record DoctorAvailabilityResponseDto(
    Guid Id,
    Guid DoctorProfileId,
    DayOfWeek DayOfWeek,
    TimeSpan StartTime,
    TimeSpan EndTime,
    bool IsAvailable = true
);