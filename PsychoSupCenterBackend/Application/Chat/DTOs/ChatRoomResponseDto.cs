
using PsychoSupCenterBackend.Domain.Enums;

namespace PsychoSupCenterBackend.Application.Chat.DTOs;

public sealed record ChatRoomResponseDto(
    Guid Id,
    ChatType Type,
    DateTime CreatedAt,
    int ParticipantCount,
    int UnreadCount,
    string? ParticipantName = null,
    string? ParticipantAvatar = null,
    string? LastMessage = null,
    DateTime? LastMessageTime = null
);