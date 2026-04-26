using FluentValidation;
using MediatR;
using PsychoSupCenterBackend.Application.Chat.DTOs;
using PsychoSupCenterBackend.Application.Common.Behaviors;
using PsychoSupCenterBackend.Application.Common.Interfaces;
using PsychoSupCenterBackend.Application.Common.Models;
using PsychoSupCenterBackend.Domain.Entities;

namespace PsychoSupCenterBackend.Application.Chat.Commands;

public static class CreateChatRoom
{
    public sealed record Command(CreateChatRoomDto Dto) : ICommand<Result<ChatRoomResponseDto>>;

    public sealed class Validator : AbstractValidator<Command>
    {
        public Validator()
        {
            RuleFor(x => x.Dto.Type).IsInEnum();
            RuleFor(x => x.Dto.ParticipantUserIds).NotEmpty().Must(ids => ids.Count >= 2);
        }
    }

    public sealed class Handler(IUnitOfWork unitOfWork) : IRequestHandler<Command, Result<ChatRoomResponseDto>>
    {
        public async Task<Result<ChatRoomResponseDto>> Handle(Command request, CancellationToken cancellationToken)
        {
            var chatRoom = new ChatRoom
            {
                Id = Guid.NewGuid(),
                Type = request.Dto.Type,
                CreatedAt = DateTime.UtcNow,
                Participants = request.Dto.ParticipantUserIds.Distinct().Select(uid => new ChatParticipant
                {
                    Id = Guid.NewGuid(),
                    UserId = uid,
                    LastReadAt = DateTime.UtcNow,
                }).ToList(),
            };

            await unitOfWork.ChatRooms.AddAsync(chatRoom, cancellationToken);
            return Result<ChatRoomResponseDto>.Success(new ChatRoomResponseDto(chatRoom.Id, chatRoom.Type, chatRoom.CreatedAt, chatRoom.Participants.Count, 0));
        }
    }
}