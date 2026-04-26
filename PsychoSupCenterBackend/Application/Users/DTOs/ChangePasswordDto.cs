namespace PsychoSupCenterBackend.Application.Users.DTOs;

public sealed record ChangePasswordDto(
    string CurrentPassword,
    string NewPassword
);