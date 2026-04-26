using PsychoSupCenterBackend.Application.Common.Models;

namespace PsychoSupCenterBackend.Application.Common.Interfaces;


public interface IRefreshTokenRepository
{
    Task SaveTokenAsync(
        RefreshToken token,
        CancellationToken cancellationToken = default);

    Task<RefreshToken?> GetByTokenAsync(
        string token,
        CancellationToken cancellationToken = default);

    Task<bool> RevokeTokenAsync(
        string token,
        CancellationToken cancellationToken = default);

    Task RevokeAllUserTokensAsync(
        Guid userId,
        CancellationToken cancellationToken = default);

    Task DeleteExpiredTokensAsync(
        CancellationToken cancellationToken = default);
}