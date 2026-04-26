
namespace PsychoSupCenterBackend.Application.DoctorAvailabilities.DTOs;

public sealed record DoctorAvailabilityResponseDto(
    Guid Id,
    Guid DoctorProfileId,
    DayOfWeek Day,
    TimeSpan StartTime,
    TimeSpan EndTime
);