
using FluentValidation;
using MediatR;
using Microsoft.EntityFrameworkCore;
using PsychoSupCenterBackend.Application.Chat.DTOs;
using PsychoSupCenterBackend.Application.Common.Behaviors;
using PsychoSupCenterBackend.Application.Common.Interfaces;
using PsychoSupCenterBackend.Application.Common.Models;

namespace PsychoSupCenterBackend.Application.Chat.Queries;

public static class GetChatMessages
{
    public sealed record Query(
        Guid ChatRoomId,
        int Page = 1,
        int PageSize = 50
    ) : IQuery<Result<IReadOnlyList<ChatMessageResponseDto>>>;

    public sealed class Validator : AbstractValidator<Query>
    {
        public Validator()
        {
            RuleFor(x => x.ChatRoomId).NotEmpty();
            RuleFor(x => x.PageSize).InclusiveBetween(1, 100);
        }
    }

    public sealed class Handler(IUnitOfWork unitOfWork)
        : IRequestHandler<Query, Result<IReadOnlyList<ChatMessageResponseDto>>>
    {
        public async Task<Result<IReadOnlyList<ChatMessageResponseDto>>> Handle(
            Query request, CancellationToken cancellationToken)
        {
            var messages = await unitOfWork.ChatMessages
                .Query()
                .Include(m => m.Sender)
                .Where(m => m.ChatRoomId == request.ChatRoomId)
                .OrderByDescending(m => m.SentAt)
                .Skip((request.Page - 1) * request.PageSize)
                .Take(request.PageSize)
                .Select(m => new ChatMessageResponseDto(
                    m.Id, m.ChatRoomId, m.SenderId,
                    m.Sender.FirstName + " " + m.Sender.LastName,
                    m.Content, m.Type, m.IsRead,
                    m.SentAt, m.EditedAt, m.IsDeleted))
                .ToListAsync(cancellationToken);

            return Result<IReadOnlyList<ChatMessageResponseDto>>.Success(messages);
        }
    }
}