
namespace PsychoSupCenterBackend.Application.DoctorAvailabilities.DTOs;

public sealed record DoctorUnavailabilityResponseDto(
    Guid Id,
    Guid DoctorProfileId,
    DateTime StartDate,
    DateTime EndDate,
    string? Reason
);