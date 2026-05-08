
namespace PsychoSupCenterBackend.Application.DoctorSpecializations.DTOs;

public sealed record SpecializationResponseDto(
    Guid Id,
    string Name,
    string Description,
    IEnumerable<Guid> DoctorProfileIds
);