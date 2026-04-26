
namespace PsychoSupCenterBackend.Application.DoctorAvailabilities.DTOs;

public sealed record CreateDoctorAvailabilityDto(
    Guid DoctorProfileId,
    DayOfWeek Day,
    TimeSpan StartTime,
    TimeSpan EndTime
);