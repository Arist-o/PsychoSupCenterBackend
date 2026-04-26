
using PsychoSupCenterBackend.Domain.Enums;

namespace PsychoSupCenterBackend.Application.Chat.DTOs;

public sealed record ChatMessageResponseDto(
    Guid Id,
    Guid ChatRoomId,
    Guid SenderId,
    string SenderName,
    string Content,
    MessageType Type,
    bool IsRead,
    DateTime SentAt,
    DateTime? EditedAt,
    bool IsDeleted
);