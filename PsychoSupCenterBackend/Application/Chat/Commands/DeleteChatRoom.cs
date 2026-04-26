
using FluentValidation;
using MediatR;
using PsychoSupCenterBackend.Application.Common.Behaviors;
using PsychoSupCenterBackend.Application.Common.Interfaces;
using PsychoSupCenterBackend.Application.Common.Models;

namespace PsychoSupCenterBackend.Application.Chat.Commands;

public static class DeleteChatRoom
{
    public sealed record Command(Guid ChatRoomId) : ICommand<Result<bool>>;

    public sealed class Validator : AbstractValidator<Command>
    {
        public Validator() => RuleFor(x => x.ChatRoomId).NotEmpty();
    }

    public sealed class Handler(IUnitOfWork unitOfWork)
        : IRequestHandler<Command, Result<bool>>
    {
        public async Task<Result<bool>> Handle(
            Command request, CancellationToken cancellationToken)
        {
            var chatRoom = await unitOfWork.ChatRooms
                .GetByIdAsync(request.ChatRoomId, cancellationToken);

            if (chatRoom is null)
                return Result<bool>.Failure("Чат-кімнату не знайдено.");

            unitOfWork.ChatRooms.Remove(chatRoom);
            return Result<bool>.Success(true);
        }
    }
}