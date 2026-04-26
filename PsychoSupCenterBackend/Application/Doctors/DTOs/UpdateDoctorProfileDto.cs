
namespace PsychoSupCenterBackend.Application.Doctors.DTOs;

public sealed record UpdateDoctorProfileDto(
    string? Bio,
    DateTime CareerStartDate
);