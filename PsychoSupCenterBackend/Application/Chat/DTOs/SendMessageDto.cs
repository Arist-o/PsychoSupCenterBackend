
using PsychoSupCenterBackend.Domain.Enums;

namespace PsychoSupCenterBackend.Application.Chat.DTOs;

public sealed record SendMessageDto(
    Guid ChatRoomId,
    Guid SenderId,
    string Content,
    MessageType Type = MessageType.Text
);