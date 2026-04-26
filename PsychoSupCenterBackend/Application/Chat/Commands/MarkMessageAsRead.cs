// Application/Chat/Commands/MarkMessageAsRead.cs
using FluentValidation;
using MediatR;
using PsychoSupCenterBackend.Application.Common.Behaviors;
using PsychoSupCenterBackend.Application.Common.Interfaces;
using PsychoSupCenterBackend.Application.Common.Models;
using Microsoft.EntityFrameworkCore;

namespace PsychoSupCenterBackend.Application.Chat.Commands;

public static class MarkMessageAsRead
{
    public sealed record Command(Guid ChatRoomId, Guid UserId)
        : ICommand<Result<bool>>;

    public sealed class Validator : AbstractValidator<Command>
    {
        public Validator()
        {
            RuleFor(x => x.ChatRoomId).NotEmpty();
            RuleFor(x => x.UserId).NotEmpty();
        }
    }

    public sealed class Handler(IUnitOfWork unitOfWork)
        : IRequestHandler<Command, Result<bool>>
    {
        public async Task<Result<bool>> Handle(
            Command request, CancellationToken cancellationToken)
        {
            var participant = await unitOfWork.ChatParticipants.FirstOrDefaultAsync(
                p => p.ChatRoomId == request.ChatRoomId && p.UserId == request.UserId,
                cancellationToken);

            if (participant is null)
                return Result<bool>.Failure("Користувач не є учасником цієї кімнати.");

            participant.LastReadAt = DateTime.UtcNow;
            unitOfWork.ChatParticipants.Update(participant);

            var unreadMessages = await unitOfWork.ChatMessages
                .Query()
                .IgnoreQueryFilters() 
                .Where(m => m.ChatRoomId == request.ChatRoomId
                         && m.SenderId != request.UserId
                         && !m.IsRead
                         && !m.IsDeleted)
                .ToListAsync(cancellationToken);

            foreach (var msg in unreadMessages)
            {
                msg.IsRead = true;
                unitOfWork.ChatMessages.Update(msg);
            }

            return Result<bool>.Success(true);
        }
    }
}