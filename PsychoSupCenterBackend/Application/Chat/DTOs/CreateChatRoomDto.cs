using PsychoSupCenterBackend.Domain.Enums;
namespace PsychoSupCenterBackend.Application.Chat.DTOs;

public sealed record CreateChatRoomDto(ChatType Type, List<Guid> ParticipantUserIds);