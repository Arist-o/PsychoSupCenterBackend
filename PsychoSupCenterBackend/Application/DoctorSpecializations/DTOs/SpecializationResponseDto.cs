
namespace PsychoSupCenterBackend.Application.DoctorSpecializations.DTOs;

public sealed record SpecializationResponseDto(
    Guid Id,
    Guid DoctorProfileId,
    string Name
);