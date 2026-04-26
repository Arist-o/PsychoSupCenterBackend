using FluentValidation;
using MediatR;
using PsychoSupCenterBackend.Application.Chat.DTOs;
using PsychoSupCenterBackend.Application.Common.Behaviors;
using PsychoSupCenterBackend.Application.Common.Interfaces;
using PsychoSupCenterBackend.Application.Common.Models;
using PsychoSupCenterBackend.Domain.Entities;

namespace PsychoSupCenterBackend.Application.Chat.Commands;

public static class SendMessage
{
    public sealed record Command(SendMessageDto Dto) : ICommand<Result<ChatMessageResponseDto>>;

    public sealed class Validator : AbstractValidator<Command>
    {
        public Validator()
        {
            RuleFor(x => x.Dto.ChatRoomId).NotEmpty();
            RuleFor(x => x.Dto.SenderId).NotEmpty();
            RuleFor(x => x.Dto.Content).NotEmpty().MaximumLength(4000);
            RuleFor(x => x.Dto.Type).IsInEnum();
        }
    }

    public sealed class Handler(IUnitOfWork unitOfWork) : IRequestHandler<Command, Result<ChatMessageResponseDto>>
    {
        public async Task<Result<ChatMessageResponseDto>> Handle(Command request, CancellationToken cancellationToken)
        {
            var message = new ChatMessage
            {
                Id = Guid.NewGuid(),
                ChatRoomId = request.Dto.ChatRoomId,
                SenderId = request.Dto.SenderId,
                Content = request.Dto.Content,
                Type = request.Dto.Type,
                IsRead = false,
                SentAt = DateTime.UtcNow,
                IsDeleted = false,
            };

            await unitOfWork.ChatMessages.AddAsync(message, cancellationToken);
            var sender = await unitOfWork.Users.GetByIdAsync(request.Dto.SenderId, cancellationToken);

            return Result<ChatMessageResponseDto>.Success(new ChatMessageResponseDto(
                message.Id, message.ChatRoomId, message.SenderId, $"{sender?.FirstName} {sender?.LastName}".Trim(),
                message.Content, message.Type, message.IsRead, message.SentAt, message.EditedAt, message.IsDeleted));
        }
    }
}