using PsychoSupCenterBackend.Domain.Enums;

namespace PsychoSupCenterBackend.Application.Doctors.DTOs;

public sealed record DoctorProfileResponseDto(
    Guid Id,
    Guid UserId,
    string FirstName,
    string LastName,
    string Email,
    string? PhotoUrl,
    string? Bio,
    DateTime CareerStartDate,
    int ExperienceYears,
    DoctorStatus Status,
    double AverageRating,
    DateTime UpdatedAt
);