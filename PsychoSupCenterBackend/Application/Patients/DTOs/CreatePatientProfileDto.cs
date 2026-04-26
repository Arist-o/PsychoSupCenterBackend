
using PsychoSupCenterBackend.Domain.Enums;

namespace PsychoSupCenterBackend.Application.Patients.DTOs;

public sealed record CreatePatientProfileDto(
    PatientType Type,
    string? MilitaryId,
    string? EmergencyContact,
    DateTime DateOfBirth
);