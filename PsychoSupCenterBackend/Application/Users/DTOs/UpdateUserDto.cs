namespace PsychoSupCenterBackend.Application.Users.DTOs;

public sealed record UpdateUserDto(
    string FirstName,
    string LastName,
    string? PhoneNumber,
    string? PhotoUrl
);