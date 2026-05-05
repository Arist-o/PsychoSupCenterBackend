namespace PsychoSupCenterBackend.Application.DoctorAvailabilities.DTOs;

public sealed record AddDoctorUnavailabilityDto(
    Guid DoctorProfileId,
    DateTime StartDate,
    DateTime EndDate,
    string? Reason
);