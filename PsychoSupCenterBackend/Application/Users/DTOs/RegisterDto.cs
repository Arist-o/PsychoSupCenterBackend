
using PsychoSupCenterBackend.Domain.Enums;

namespace PsychoSupCenterBackend.Application.Users.DTOs;

public sealed record RegisterDto(
    string Email,
    string Password,
    string FirstName,
    string LastName,
    int Age,
    string? PhoneNumber,
    PatientType? PatientType,
    UserRole Role
);