namespace PsychoSupCenterBackend.Application.Common.Models;


public sealed class RefreshToken
{
    public string Id { get; set; } = string.Empty;

    public string Token { get; set; } = string.Empty;

    public Guid UserId { get; set; }

    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public DateTime ExpiresAt { get; set; }
    public bool IsRevoked { get; set; } = false;

    public string? CreatedByIp { get; set; }
}