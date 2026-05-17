using PsychoSupCenterBackend.Domain.Enums;

namespace PsychoSupCenterBackend.Application.Doctors.DTOs;

public sealed record AssignSpecializationsDto(List<Guid> SpecializationIds);