
using FluentValidation;
using MediatR;
using PsychoSupCenterBackend.Application.Common.Behaviors;
using PsychoSupCenterBackend.Application.Common.Interfaces;
using PsychoSupCenterBackend.Application.Common.Models;
using PsychoSupCenterBackend.Application.Users.DTOs;

namespace PsychoSupCenterBackend.Application.Users.Commands;

public static class RefreshTokenCommand
{
   
    public sealed record Command(
        string RefreshToken,
        string? IpAddress = null
    ) : ICommand<Result<AuthResponseDto>>;

   
    public sealed class Validator : AbstractValidator<Command>
    {
        public Validator()
        {
            RuleFor(x => x.RefreshToken)
                .NotEmpty().WithMessage("RefreshToken є обов'язковим.");
        }
    }

 
    public sealed class Handler(
        IUnitOfWork unitOfWork,
        IRefreshTokenRepository refreshTokenRepository,
        IJwtTokenService jwtTokenService)
        : IRequestHandler<Command, Result<AuthResponseDto>>
    {
        public async Task<Result<AuthResponseDto>> Handle(
            Command request,
            CancellationToken cancellationToken)
        {
          
            var storedToken = await refreshTokenRepository
                .GetByTokenAsync(request.RefreshToken, cancellationToken);

            if (storedToken is null)
                return Result<AuthResponseDto>.Failure("Недійсний refresh token.");

            if (storedToken.IsRevoked)
                return Result<AuthResponseDto>.Failure("Refresh token відкликано.");

            if (storedToken.ExpiresAt < DateTime.UtcNow)
                return Result<AuthResponseDto>.Failure("Refresh token прострочено.");

           
            var user = await unitOfWork.Users.FirstOrDefaultAsync(
                u => u.Id == storedToken.UserId && u.IsActive,
                cancellationToken);

            if (user is null)
                return Result<AuthResponseDto>.Failure(
                    "Користувача не знайдено або акаунт деактивовано.");

          
            await refreshTokenRepository.RevokeTokenAsync(
                request.RefreshToken, cancellationToken);

            var newAccessToken = jwtTokenService.GenerateAccessToken(user);
            var newRefreshTokenValue = jwtTokenService.GenerateRefreshToken();

            var newRefreshToken = new RefreshToken
            {
                Id = Guid.NewGuid().ToString(),
                Token = newRefreshTokenValue,
                UserId = user.Id,
                CreatedAt = DateTime.UtcNow,
                ExpiresAt = DateTime.UtcNow.Add(jwtTokenService.RefreshTokenLifetime),
                IsRevoked = false,
                CreatedByIp = request.IpAddress,
            };

            await refreshTokenRepository.SaveTokenAsync(newRefreshToken, cancellationToken);

        
            return Result<AuthResponseDto>.Success(new AuthResponseDto(
                UserId: user.Id,
                Email: user.Email,
                FirstName: user.FirstName,
                LastName: user.LastName,
                Role: user.Role.ToString(),
                AccessToken: newAccessToken,
                RefreshToken: newRefreshTokenValue,
                AccessTokenExpiresAt: DateTime.UtcNow.AddMinutes(15)
            ));
        }
    }
}