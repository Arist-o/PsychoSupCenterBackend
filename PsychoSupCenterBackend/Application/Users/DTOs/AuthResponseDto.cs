namespace PsychoSupCenterBackend.Application.Users.DTOs;

public sealed record AuthResponseDto(
    Guid UserId,
    string Email,
    string FirstName,
    string LastName,
    string Role,
    string AccessToken,
    string RefreshToken,
    DateTime AccessTokenExpiresAt
);