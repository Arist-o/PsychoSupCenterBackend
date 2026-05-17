namespace PsychoSupCenterBackend.Application.Admin.DTOs;

public sealed record CreateAdminDto(
    string Email,
    string FirstName,
    string LastName,
    int Age,
    string Password
);