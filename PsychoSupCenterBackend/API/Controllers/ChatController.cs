using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using PsychoSupCenterBackend.Application.Chat.Commands;
using PsychoSupCenterBackend.Application.Chat.Queries;
using PsychoSupCenterBackend.Application.Chat.DTOs;
using System.Security.Claims;

namespace PsychoSupCenterBackend.API.Controllers;

[Authorize]
public class ChatController : BaseApiController
{
    private Guid GetCurrentUserId()
    {
        var claim = User.FindFirstValue(ClaimTypes.NameIdentifier);
        return Guid.TryParse(claim, out var userId) ? userId : Guid.Empty;
    }

  
    [HttpGet("rooms/{id}")]
    public async Task<ActionResult<ChatRoomResponseDto>> GetRoomById(Guid id)
    {
        return HandleResult(await Mediator.Send(
            new GetChatRoomById.Query(id, GetCurrentUserId())));
    }

    [HttpGet("rooms/my")]
    public async Task<ActionResult<IReadOnlyList<ChatRoomResponseDto>>> GetMyRooms()
    {
        return HandleResult(await Mediator.Send(
            new GetUserChatRooms.Query(GetCurrentUserId())));
    }

    [HttpPost("rooms")]
    public async Task<ActionResult<ChatRoomResponseDto>> CreateRoom(
        [FromBody] CreateChatRoomDto dto)
    {
        return HandleResult(await Mediator.Send(new CreateChatRoom.Command(dto)));
    }

    [HttpDelete("rooms/{id}")]
    public async Task<ActionResult<bool>> DeleteRoom(Guid id)
    {
        return HandleResult(await Mediator.Send(new DeleteChatRoom.Command(id)));
    }

    
    [HttpGet("rooms/{roomId}/messages")]
    public async Task<ActionResult<IReadOnlyList<ChatMessageResponseDto>>> GetMessages(
        Guid roomId,
        [FromQuery] int page = 1,
        [FromQuery] int pageSize = 50)
    {
        return HandleResult(await Mediator.Send(
            new GetChatMessages.Query(roomId, page, pageSize)));
    }

    
}