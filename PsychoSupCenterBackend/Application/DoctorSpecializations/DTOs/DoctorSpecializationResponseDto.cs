namespace PsychoSupCenterBackend.Application.DoctorSpecializations.DTOs;

public sealed record DoctorSpecializationResponseDto(
    Guid Id,
    string Name,
    string Description
);