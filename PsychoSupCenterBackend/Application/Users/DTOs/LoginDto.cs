
namespace PsychoSupCenterBackend.Application.Users.DTOs;

public sealed record LoginDto(
    string Email,
    string Password
);