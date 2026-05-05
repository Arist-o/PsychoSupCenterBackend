namespace PsychoSupCenterBackend.Application.DoctorSpecializations.DTOs;

public sealed record RemoveSpecializationDto(Guid DoctorProfileId, string Name);
