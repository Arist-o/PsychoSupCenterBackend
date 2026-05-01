using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using PsychoSupCenterBackend.Application.Chat.Commands;
using PsychoSupCenterBackend.Application.Chat.Queries;
using PsychoSupCenterBackend.Application.Chat.DTOs;
using Application.Chat.DTOs;

namespace PsychoSupCenterBackend.API.Controllers;

[Authorize]
public class ChatController : BaseApiController
{
    private Guid GetCurrentUserId()
    {
        var userIdClaim = User.FindFirstValue(ClaimTypes.NameIdentifier);
        return Guid.TryParse(userIdClaim, out var userId) ? userId : Guid.Empty;
    }

    [HttpGet("rooms/{id}")]
    public async Task<ActionResult<ChatRoomResponseDto>> GetRoomById(Guid id)
    {
        var currentUserId = GetCurrentUserId();
        return HandleResult(await Mediator.Send(new GetChatRoomById.Query(id, currentUserId)));
    }

    [HttpGet("rooms/my")] 
    public async Task<ActionResult<IReadOnlyList<ChatRoomResponseDto>>> GetMyRooms()
    {
        var currentUserId = GetCurrentUserId();
        return HandleResult(await Mediator.Send(new GetUserChatRooms.Query(currentUserId)));
    }

    [HttpPost("rooms")]
    public async Task<ActionResult<ChatRoomResponseDto>> CreateRoom([FromBody] CreateChatRoomDto dto)
    {
        return HandleResult(await Mediator.Send(new CreateChatRoom.Command(dto)));
    }

    [HttpDelete("rooms/{id}")]
    public async Task<ActionResult<bool>> DeleteRoom(Guid id)
    {
        return HandleResult(await Mediator.Send(new DeleteChatRoom.Command(id)));
    }

 
    [HttpGet("rooms/{roomId}/messages")]
    public async Task<ActionResult<IReadOnlyList<ChatMessageResponseDto>>> GetMessages(Guid roomId, [FromQuery] int page = 1, [FromQuery] int pageSize = 50)
    {
        return HandleResult(await Mediator.Send(new GetChatMessages.Query(roomId, page, pageSize)));
    }

    [HttpPost("messages")]
    public async Task<ActionResult<ChatMessageResponseDto>> SendMessage([FromBody] SendMessageDto dto)
    {
        var safeDto = dto with { SenderId = GetCurrentUserId() };
        return HandleResult(await Mediator.Send(new SendMessage.Command(safeDto)));
    }

    [HttpPut("messages/{messageId}")]
    public async Task<ActionResult<ChatMessageResponseDto>> EditMessage(Guid messageId, [FromBody] EditMessageDto dto)
    {
        var safeDto = dto with { EditorUserId = GetCurrentUserId() };
        return HandleResult(await Mediator.Send(new EditMessage.Command(messageId, safeDto)));
    }

    [HttpPatch("messages/{messageId}/read")]
    public async Task<ActionResult<bool>> MarkAsRead(Guid messageId)
    {
        return HandleResult(await Mediator.Send(new MarkMessageAsRead.Command(messageId, GetCurrentUserId())));
    }

    [HttpDelete("messages/{messageId}")]
    public async Task<ActionResult<bool>> DeleteMessage(Guid messageId)
    {
        return HandleResult(await Mediator.Send(new DeleteMessage.Command(messageId, GetCurrentUserId())));
    }
}