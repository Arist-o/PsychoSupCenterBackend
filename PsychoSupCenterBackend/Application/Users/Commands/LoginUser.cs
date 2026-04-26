using FluentValidation;
using MediatR;
using Microsoft.AspNetCore.Identity;
using PsychoSupCenterBackend.Application.Common.Behaviors;
using PsychoSupCenterBackend.Application.Common.Interfaces;
using PsychoSupCenterBackend.Application.Common.Models;
using PsychoSupCenterBackend.Application.Users.DTOs;
using PsychoSupCenterBackend.Domain.Entities;

namespace PsychoSupCenterBackend.Application.Users.Commands;

public static class LoginUser
{
    public sealed record Command(LoginDto Dto, string? IpAddress = null) : ICommand<Result<AuthResponseDto>>;

    public sealed class Validator : AbstractValidator<Command>
    {
        public Validator()
        {
            RuleFor(x => x.Dto.Email)
                .NotEmpty().WithMessage("Email є обов'язковим.")
                .EmailAddress().WithMessage("Некоректний формат Email.");

            RuleFor(x => x.Dto.Password)
                .NotEmpty().WithMessage("Пароль є обов'язковим.");
        }
    }

    public sealed class Handler(
        IUnitOfWork unitOfWork,
        IRefreshTokenRepository refreshTokenRepository,
        IJwtTokenService jwtTokenService,
        IPasswordHasher<ApplicationUser> passwordHasher)
        : IRequestHandler<Command, Result<AuthResponseDto>>
    {
        public async Task<Result<AuthResponseDto>> Handle(
            Command request, CancellationToken cancellationToken)
        {
            var user = await unitOfWork.Users.FirstOrDefaultAsync(
                u => u.Email == request.Dto.Email.ToLowerInvariant() && u.IsActive,
                cancellationToken);

            if (user is null)
                return Result<AuthResponseDto>.Failure("Невірний Email або пароль.");

            var passwordResult = passwordHasher.VerifyHashedPassword(user, user.PasswordHash, request.Dto.Password);

            if (passwordResult == PasswordVerificationResult.Failed)
                return Result<AuthResponseDto>.Failure("Невірний Email або пароль.");

            await refreshTokenRepository.RevokeAllUserTokensAsync(user.Id, cancellationToken);

            var accessToken = jwtTokenService.GenerateAccessToken(user);
            var refreshTokenValue = jwtTokenService.GenerateRefreshToken();

            var refreshToken = new RefreshToken
            {
                Id = Guid.NewGuid().ToString(),
                Token = refreshTokenValue,
                UserId = user.Id,
                CreatedAt = DateTime.UtcNow,
                ExpiresAt = DateTime.UtcNow.Add(jwtTokenService.RefreshTokenLifetime),
                IsRevoked = false,
                CreatedByIp = request.IpAddress,
            };

            await refreshTokenRepository.SaveTokenAsync(refreshToken, cancellationToken);

            return Result<AuthResponseDto>.Success(new AuthResponseDto(
                UserId: user.Id,
                Email: user.Email,
                FirstName: user.FirstName,
                LastName: user.LastName,
                Role: user.Role.ToString(),
                AccessToken: accessToken,
                RefreshToken: refreshTokenValue,
                AccessTokenExpiresAt: DateTime.UtcNow.AddMinutes(15)
            ));
        }
    }
}