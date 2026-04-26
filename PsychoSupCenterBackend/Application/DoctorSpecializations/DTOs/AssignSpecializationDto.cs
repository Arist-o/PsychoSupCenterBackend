
namespace PsychoSupCenterBackend.Application.DoctorSpecializations.DTOs;

public sealed record AssignSpecializationDto(
    Guid DoctorProfileId,
    string Name
);