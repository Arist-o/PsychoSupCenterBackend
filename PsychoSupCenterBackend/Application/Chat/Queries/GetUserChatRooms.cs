
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
                .Include(p => p.ChatRoom)
                    .ThenInclude(r => r.Participants)
                .Where(p => p.UserId == request.UserId)
                .OrderByDescending(p => p.ChatRoom.CreatedAt)
                .Select(p => new ChatRoomResponseDto(
                    p.ChatRoom.Id,
                    p.ChatRoom.Type,
                    p.ChatRoom.CreatedAt,
                    p.ChatRoom.Participants.Count,
                    p.ChatRoom.Messages.Count(m =>
                        m.SenderId != request.UserId && !m.IsRead && !m.IsDeleted)))
                .ToListAsync(cancellationToken);

            return Result<IReadOnlyList<ChatRoomResponseDto>>.Success(rooms);
        }
    }
}