using Microsoft.AspNetCore.SignalR;
using System.Security.Claims;

namespace PsychoSupCenterBackend.Infrasructure.Auth;

public class SignalRUserIdProvider : IUserIdProvider
{
    public string? GetUserId(HubConnectionContext connection)
    {
        return connection.User?.FindFirstValue(ClaimTypes.NameIdentifier);
    }
}