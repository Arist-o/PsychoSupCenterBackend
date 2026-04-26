using PsychoSupCenterBackend.Domain.Entities;

namespace PsychoSupCenterBackend.Application.Common.Interfaces;

public interface IJwtTokenService
{
    string GenerateAccessToken(ApplicationUser user);

    string GenerateRefreshToken();

    TimeSpan RefreshTokenLifetime { get; }
}