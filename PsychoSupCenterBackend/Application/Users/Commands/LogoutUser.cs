
using FluentValidation;
using MediatR;
using PsychoSupCenterBackend.Application.Common.Behaviors;
using PsychoSupCenterBackend.Application.Common.Interfaces;
using PsychoSupCenterBackend.Application.Common.Models;

namespace PsychoSupCenterBackend.Application.Users.Commands;

public static class LogoutUser
{
    public sealed record Command(string RefreshToken) : ICommand<Result<bool>>;

    public sealed class Validator : AbstractValidator<Command>
    {
        public Validator()
        {
            RuleFor(x => x.RefreshToken)
                .NotEmpty().WithMessage("RefreshToken є обов'язковим.");
        }
    }

    public sealed class Handler(IRefreshTokenRepository refreshTokenRepository)
        : IRequestHandler<Command, Result<bool>>
    {
        public async Task<Result<bool>> Handle(
            Command request,
            CancellationToken cancellationToken)
        {
            var revoked = await refreshTokenRepository
                .RevokeTokenAsync(request.RefreshToken, cancellationToken);

            return revoked
                ? Result<bool>.Success(true)
                : Result<bool>.Failure("Токен не знайдено або вже відкликано.");
        }
    }
}