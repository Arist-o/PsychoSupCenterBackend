namespace PsychoSupCenterBackend.Application.Admin.DTOs;

public sealed record CreateAdminDto(
    string Email,
    string FirstName,
    string LastName,
    string Password
);