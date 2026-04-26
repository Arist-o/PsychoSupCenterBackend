
using PsychoSupCenterBackend.Domain.Enums;

namespace PsychoSupCenterBackend.Application.Patients.DTOs;

public sealed record PatientProfileResponseDto(
    Guid Id,
    Guid UserId,
    string FirstName,
    string LastName,
    string Email,
    string? PhotoUrl,
    PatientType Type,
    string? MilitaryId,
    string? EmergencyContact,
    DateTime DateOfBirth,
    int Age
);