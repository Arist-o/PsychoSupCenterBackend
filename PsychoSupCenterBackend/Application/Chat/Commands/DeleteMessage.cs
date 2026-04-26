
using FluentValidation;
using MediatR;
using PsychoSupCenterBackend.Application.Common.Behaviors;
using PsychoSupCenterBackend.Application.Common.Interfaces;
using PsychoSupCenterBackend.Application.Common.Models;

namespace PsychoSupCenterBackend.Application.Chat.Commands;

public static class DeleteMessage
{
    public sealed record Command(Guid MessageId, Guid RequestingUserId)
        : ICommand<Result<bool>>;

    public sealed class Validator : AbstractValidator<Command>
    {
        public Validator()
        {
            RuleFor(x => x.MessageId).NotEmpty();
            RuleFor(x => x.RequestingUserId).NotEmpty();
        }
    }

    public sealed class Handler(IUnitOfWork unitOfWork)
        : IRequestHandler<Command, Result<bool>>
    {
        public async Task<Result<bool>> Handle(
            Command request, CancellationToken cancellationToken)
        {
            var message = await unitOfWork.ChatMessages
                .GetByIdAsync(request.MessageId, cancellationToken);

            if (message is null || message.IsDeleted)
                return Result<bool>.Failure("Повідомлення не знайдено.");

            if (message.SenderId != request.RequestingUserId)
                return Result<bool>.Failure(
                    "Можна видаляти лише власні повідомлення.");

            message.IsDeleted = true;
            message.Content = "[Повідомлення видалено]";
            unitOfWork.ChatMessages.Update(message);

            return Result<bool>.Success(true);
        }
    }
}