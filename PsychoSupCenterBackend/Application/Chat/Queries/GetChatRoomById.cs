
using FluentValidation;
using MediatR;
using Microsoft.EntityFrameworkCore;
using PsychoSupCenterBackend.Application.Chat.DTOs;
using PsychoSupCenterBackend.Application.Common.Behaviors;
using PsychoSupCenterBackend.Application.Common.Interfaces;
using PsychoSupCenterBackend.Application.Common.Models;

namespace PsychoSupCenterBackend.Application.Chat.Queries;

public static class GetChatRoomById
{
    public sealed record Query(Guid ChatRoomId, Guid CurrentUserId)
        : IQuery<Result<ChatRoomResponseDto>>;

    public sealed class Validator : AbstractValidator<Query>
    {
        public Validator() => RuleFor(x => x.ChatRoomId).NotEmpty();
    }

    public sealed class Handler(IUnitOfWork unitOfWork)
        : IRequestHandler<Query, Result<ChatRoomResponseDto>>
    {
        public async Task<Result<ChatRoomResponseDto>> Handle(
            Query request, CancellationToken cancellationToken)
        {
            var chatRoom = await unitOfWork.ChatRooms
                .Query()
                .Include(r => r.Participants)
                .FirstOrDefaultAsync(r => r.Id == request.ChatRoomId, cancellationToken);

            if (chatRoom is null)
                return Result<ChatRoomResponseDto>.Failure("Чат-кімнату не знайдено.");

            var userParticipant = chatRoom.Participants
                .FirstOrDefault(p => p.UserId == request.CurrentUserId);

            var unreadCount = await unitOfWork.ChatMessages
                .Query()
                .CountAsync(m => m.ChatRoomId == request.ChatRoomId
                              && m.SenderId != request.CurrentUserId
                              && !m.IsRead
                              && !m.IsDeleted
                              && (userParticipant == null
                                  || m.SentAt > userParticipant.LastReadAt),
                    cancellationToken);

            return Result<ChatRoomResponseDto>.Success(new ChatRoomResponseDto(
                chatRoom.Id, chatRoom.Type, chatRoom.CreatedAt,
                chatRoom.Participants.Count, unreadCount));
        }
    }
}