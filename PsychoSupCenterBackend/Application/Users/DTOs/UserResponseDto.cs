namespace PsychoSupCenterBackend.Application.Users.DTOs;

public sealed record UserResponseDto(
    Guid Id,
    string Email,
    string FirstName,
    string LastName,
    int Age,
    string PhoneNumber,
    string? PhotoUrl,
    string Role,
    bool IsActive,
    Guid? DoctorProfileId = null,
    Guid? PatientProfileId = null
);