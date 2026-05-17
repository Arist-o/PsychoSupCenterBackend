namespace PsychoSupCenterBackend.Application.Common.Interfaces;

public interface ICurrentUserAccessor
{
    string? UserId { get; }
    string? UserRole { get; }
    bool IsAuthenticated { get; }
}