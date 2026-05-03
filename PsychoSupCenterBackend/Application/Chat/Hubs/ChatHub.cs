using Application.Chat.DTOs;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.SignalR;
using Microsoft.EntityFrameworkCore;
using PsychoSupCenterBackend.Application.Chat.Commands;
using PsychoSupCenterBackend.Application.Chat.DTOs;
using System.Security.Claims;
using System.Text.RegularExpressions;

namespace PsychoSupCenterBackend.Application.Chat.Hubs;

[Authorize]
public class ChatHub : Hub
{
    private readonly IMediator _mediator;

    public ChatHub(IMediator mediator)
    {
        _mediator = mediator;
    }

    private Guid GetCurrentUserId()
    {
        var claim = Context.User?.FindFirstValue(ClaimTypes.NameIdentifier);
        return Guid.TryParse(claim, out var id) ? id : Guid.Empty;
    }

    public async Task JoinRoom(Guid roomId)
    {
        await Groups.AddToGroupAsync(Context.ConnectionId, roomId.ToString());
    }

    public async Task LeaveRoom(Guid roomId)
    {
        await Groups.RemoveFromGroupAsync(Context.ConnectionId, roomId.ToString());
    }

    public async Task SendMessage(SendMessageDto dto)
    {
        var safeDto = dto with { SenderId = GetCurrentUserId() };
        var result = await _mediator.Send(new SendMessage.Command(safeDto));

        if (!result.IsSuccess)
        {
            await Clients.Caller.SendAsync("Error", result.Error);
            return;
        }

        await Clients.Group(dto.ChatRoomId.ToString())
            .SendAsync("ReceiveMessage", result.Value);
    }

    public async Task EditMessage(Guid messageId, EditMessageDto dto)
    {
        var safeDto = dto with { EditorUserId = GetCurrentUserId() };
        var result = await _mediator.Send(new EditMessage.Command(messageId, safeDto));

        if (!result.IsSuccess)
        {
            await Clients.Caller.SendAsync("Error", result.Error);
            return;
        }

        await Clients.Group(result.Value!.ChatRoomId.ToString())
            .SendAsync("MessageEdited", result.Value);
    }

    public async Task DeleteMessage(Guid messageId, Guid chatRoomId)
    {
        var result = await _mediator.Send(
            new DeleteMessage.Command(messageId, GetCurrentUserId()));

        if (!result.IsSuccess)
        {
            await Clients.Caller.SendAsync("Error", result.Error);
            return;
        }

        await Clients.Group(chatRoomId.ToString())
            .SendAsync("MessageDeleted", messageId);
    }

    public async Task MarkAsRead(Guid chatRoomId)
    {
        var result = await _mediator.Send(
            new MarkMessageAsRead.Command(chatRoomId, GetCurrentUserId()));

        if (!result.IsSuccess)
        {
            await Clients.Caller.SendAsync("Error", result.Error);
            return;
        }

        await Clients.Group(chatRoomId.ToString())
            .SendAsync("MessagesRead", new { ChatRoomId = chatRoomId, UserId = GetCurrentUserId() });
    }

    public override async Task OnConnectedAsync()
    {
        await base.OnConnectedAsync();
    }

    public override async Task OnDisconnectedAsync(Exception? exception)
    {
        await base.OnDisconnectedAsync(exception);
    }
}