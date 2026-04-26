namespace PsychoSupCenterBackend.Application.Doctors.DTOs;

public sealed record CreateDoctorProfileDto(
    string? Bio,
    DateTime CareerStartDate
);