using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.SignalR;
using Microsoft.EntityFrameworkCore;
using PsychoSupCenterBackend.Application.Chat.Commands;
using PsychoSupCenterBackend.Application.Chat.DTOs;
using PsychoSupCenterBackend.Application.Common.Interfaces;
using System.Security.Claims;

namespace PsychoSupCenterBackend.Application.Chat.Hubs;

[Authorize]
public class ChatHub : Hub
{
    private readonly IMediator _mediator;
    private readonly IUnitOfWork _unitOfWork;

    public ChatHub(IMediator mediator, IUnitOfWork unitOfWork)
    {
        _mediator = mediator;
        _unitOfWork = unitOfWork;
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

    private async Task<List<string>> GetParticipantUserIdsAsync(Guid chatRoomId)
    {
        var participants = await _unitOfWork.ChatParticipants
            .Query()
            .Where(p => p.ChatRoomId == chatRoomId)
            .Select(p => p.UserId.ToString())
            .ToListAsync();
        
        return participants;
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

        var participantIds = await GetParticipantUserIdsAsync(dto.ChatRoomId);

        // 1. Send to the Group (Fastest for users currently IN the chat window)
        await Clients.Group(dto.ChatRoomId.ToString())
            .SendAsync("ReceiveMessage", result.Value);

        // 2. Send to specific Users (For global notifications/toasts if they are outside the chat)
        // Note: SignalR filters out duplicates if a user is in the group and targeted directly.
        await Clients.Users(participantIds)
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

        var participantIds = await GetParticipantUserIdsAsync(result.Value!.ChatRoomId);
        var roomIdStr = result.Value.ChatRoomId.ToString();

        await Clients.Group(roomIdStr).SendAsync("MessageEdited", result.Value);
        await Clients.Users(participantIds).SendAsync("MessageEdited", result.Value);
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

        var participantIds = await GetParticipantUserIdsAsync(chatRoomId);
        var roomIdStr = chatRoomId.ToString();

        await Clients.Group(roomIdStr).SendAsync("MessageDeleted", messageId);
        await Clients.Users(participantIds).SendAsync("MessageDeleted", messageId);
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

        var participantIds = await GetParticipantUserIdsAsync(chatRoomId);
        var roomIdStr = chatRoomId.ToString();
        var payload = new { ChatRoomId = chatRoomId, UserId = GetCurrentUserId() };

        await Clients.Group(roomIdStr).SendAsync("MessagesRead", payload);
        await Clients.Users(participantIds).SendAsync("MessagesRead", payload);
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