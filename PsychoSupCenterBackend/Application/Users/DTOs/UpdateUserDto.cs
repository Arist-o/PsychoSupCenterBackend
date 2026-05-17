namespace PsychoSupCenterBackend.Application.Users.DTOs;

public sealed record UpdateUserDto(
    string FirstName,
    string LastName,
    int Age,
    string? PhoneNumber,
    string? PhotoUrl
);