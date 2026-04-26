using FluentValidation;
using MediatR;
using Microsoft.AspNetCore.Identity;
using PsychoSupCenterBackend.Application.Common.Behaviors;
using PsychoSupCenterBackend.Application.Common.Interfaces;
using PsychoSupCenterBackend.Application.Common.Models;
using PsychoSupCenterBackend.Application.Users.DTOs;
using PsychoSupCenterBackend.Domain.Entities;

namespace PsychoSupCenterBackend.Application.Users.Commands;

public static class ChangePassword
{
    public sealed record Command(Guid UserId, ChangePasswordDto Dto) : ICommand<Result<bool>>;

    public sealed class Validator : AbstractValidator<Command>
    {
        public Validator()
        {
            RuleFor(x => x.UserId).NotEmpty();
            RuleFor(x => x.Dto.CurrentPassword).NotEmpty();
            RuleFor(x => x.Dto.NewPassword)
                .NotEmpty()
                .MinimumLength(8)
                .Matches(@"[A-Z]").WithMessage("Пароль має містити велику літеру.")
                .Matches(@"[0-9]").WithMessage("Пароль має містити цифру.");
        }
    }

    public sealed class Handler(
        IUnitOfWork unitOfWork,
        IPasswordHasher<ApplicationUser> passwordHasher)
        : IRequestHandler<Command, Result<bool>>
    {
        public async Task<Result<bool>> Handle(Command request, CancellationToken cancellationToken)
        {
            var user = await unitOfWork.Users.GetByIdAsync(request.UserId, cancellationToken);
            if (user is null) return Result<bool>.Failure("Користувача не знайдено.");

            var verificationResult = passwordHasher.VerifyHashedPassword(user, user.PasswordHash, request.Dto.CurrentPassword);
            if (verificationResult == PasswordVerificationResult.Failed)
                return Result<bool>.Failure("Поточний пароль невірний.");

            user.PasswordHash = passwordHasher.HashPassword(user, request.Dto.NewPassword);
            unitOfWork.Users.Update(user);

            return Result<bool>.Success(true);
        }
    }
}