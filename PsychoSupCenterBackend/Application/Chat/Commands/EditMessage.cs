
using FluentValidation;
using MediatR;
using PsychoSupCenterBackend.Application.Chat.DTOs;
using PsychoSupCenterBackend.Application.Common.Behaviors;
using PsychoSupCenterBackend.Application.Common.Interfaces;
using PsychoSupCenterBackend.Application.Common.Models;

namespace PsychoSupCenterBackend.Application.Chat.Commands;

public static class EditMessage
{
    public sealed record Command(
        Guid MessageId,
        Guid EditorUserId,
        string NewContent
    ) : ICommand<Result<ChatMessageResponseDto>>;

    public sealed class Validator : AbstractValidator<Command>
    {
        public Validator()
        {
            RuleFor(x => x.MessageId).NotEmpty();
            RuleFor(x => x.EditorUserId).NotEmpty();
            RuleFor(x => x.NewContent)
                .NotEmpty().MaximumLength(4000);
        }
    }

    public sealed class Handler(IUnitOfWork unitOfWork)
        : IRequestHandler<Command, Result<ChatMessageResponseDto>>
    {
        public async Task<Result<ChatMessageResponseDto>> Handle(
            Command request, CancellationToken cancellationToken)
        {
            var message = await unitOfWork.ChatMessages
                .GetByIdAsync(request.MessageId, cancellationToken);

            if (message is null || message.IsDeleted)
                return Result<ChatMessageResponseDto>.Failure("Повідомлення не знайдено.");

            if (message.SenderId != request.EditorUserId)
                return Result<ChatMessageResponseDto>.Failure(
                    "Можна редагувати лише власні повідомлення.");

            message.Content = request.NewContent;
            message.EditedAt = DateTime.UtcNow;
            unitOfWork.ChatMessages.Update(message);

            var sender = await unitOfWork.Users
                .GetByIdAsync(message.SenderId, cancellationToken);

            return Result<ChatMessageResponseDto>.Success(new ChatMessageResponseDto(
                message.Id, message.ChatRoomId, message.SenderId,
                $"{sender?.FirstName} {sender?.LastName}".Trim(),
                message.Content, message.Type, message.IsRead,
                message.SentAt, message.EditedAt, message.IsDeleted));
        }
    }
}