using FluentValidation;
using MediatR;
using PsychoSupCenterBackend.Application.Common.Behaviors;
using PsychoSupCenterBackend.Application.Common.Interfaces;
using PsychoSupCenterBackend.Application.Common.Models;

namespace PsychoSupCenterBackend.Application.Users.Commands;

public static class DeleteUser
{
    public sealed record Command(Guid UserId) : ICommand<Result<bool>>;

    public sealed class Validator : AbstractValidator<Command>
    {
        public Validator() => RuleFor(x => x.UserId).NotEmpty();
    }

    public sealed class Handler(IUnitOfWork unitOfWork) : IRequestHandler<Command, Result<bool>>
    {
        public async Task<Result<bool>> Handle(Command request, CancellationToken cancellationToken)
        {
            var user = await unitOfWork.Users.GetByIdAsync(request.UserId, cancellationToken);
            if (user is null) return Result<bool>.Failure("Користувача не знайдено.");

            user.IsActive = false;
            user.UpdatedAt = DateTime.UtcNow;

            unitOfWork.Users.Update(user);
            return Result<bool>.Success(true);
        }
    }
}