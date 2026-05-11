
using FluentValidation;
using MediatR;
using Microsoft.EntityFrameworkCore;
using PsychoSupCenterBackend.Application.Chat.DTOs;
using PsychoSupCenterBackend.Application.Common.Behaviors;
using PsychoSupCenterBackend.Application.Common.Interfaces;
using PsychoSupCenterBackend.Application.Common.Models;

namespace PsychoSupCenterBackend.Application.Chat.Queries;

public static class GetUserChatRooms
{
    public sealed record Query(Guid UserId)
        : IQuery<Result<IReadOnlyList<ChatRoomResponseDto>>>;

    public sealed class Validator : AbstractValidator<Query>
    {
        public Validator() => RuleFor(x => x.UserId).NotEmpty();
    }

    public sealed class Handler(IUnitOfWork unitOfWork)
        : IRequestHandler<Query, Result<IReadOnlyList<ChatRoomResponseDto>>>
    {
        public async Task<Result<IReadOnlyList<ChatRoomResponseDto>>> Handle(
            Query request, CancellationToken cancellationToken)
        {
            var rooms = await unitOfWork.ChatParticipants
                .Query()
                .Where(p => p.UserId == request.UserId)
                .Select(p => new ChatRoomResponseDto(
                    p.ChatRoom.Id,
                    p.ChatRoom.Type,
                    p.ChatRoom.CreatedAt,
                    p.ChatRoom.Participants.Count,
                    p.ChatRoom.Messages.Count(m =>
                        m.SenderId != request.UserId && !m.IsRead && !m.IsDeleted),
                    p.ChatRoom.Participants
                        .Where(op => op.UserId != request.UserId)
                        .Select(op => (op.User.FirstName + " " + op.User.LastName).Trim())
                        .FirstOrDefault(),
                    p.ChatRoom.Participants
                        .Where(op => op.UserId != request.UserId)
                        .Select(op => op.User.PhotoUrl)
                        .FirstOrDefault(),
                    p.ChatRoom.Messages
                        .Where(m => !m.IsDeleted)
                        .OrderByDescending(m => m.SentAt)
                        .Select(m => m.Content)
                        .FirstOrDefault(),
                    p.ChatRoom.Messages
                        .Where(m => !m.IsDeleted)
                        .OrderByDescending(m => m.SentAt)
                        .Select(m => (DateTime?)m.SentAt)
                        .FirstOrDefault()
                ))
                .ToListAsync(cancellationToken);

            var result = rooms
                .OrderByDescending(r => r.LastMessageTime ?? r.CreatedAt)
                .ToList();

            return Result<IReadOnlyList<ChatRoomResponseDto>>.Success(result);
        }
    }
}