namespace PsychoSupCenterBackend.Application.DoctorAvailabilities.DTOs;

public sealed record UpdateDoctorAvailabilityDto(
    DayOfWeek Day,
    TimeSpan StartTime,
    TimeSpan EndTime
);