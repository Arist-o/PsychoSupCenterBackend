using FluentValidation;
using MediatR;
using Microsoft.AspNetCore.Identity;
using PsychoSupCenterBackend.Application.Common.Behaviors;
using PsychoSupCenterBackend.Application.Common.Interfaces;
using PsychoSupCenterBackend.Application.Common.Models;
using PsychoSupCenterBackend.Application.Users.DTOs;
using PsychoSupCenterBackend.Domain.Entities;
using PsychoSupCenterBackend.Domain.Enums;

namespace PsychoSupCenterBackend.Application.Users.Commands;

public static class RegisterUser
{
    public sealed record Command(RegisterDto Dto, string? IpAddress = null) : ICommand<Result<AuthResponseDto>>;

    public sealed class Validator : AbstractValidator<Command>
    {
        public Validator()
        {
            RuleFor(x => x.Dto.Email)
                .NotEmpty().WithMessage("Email є обов'язковим.")
                .EmailAddress().WithMessage("Некоректний формат Email.")
                .MaximumLength(256);

            RuleFor(x => x.Dto.Password)
                .NotEmpty().WithMessage("Пароль є обов'язковим.")
                .MinimumLength(8).WithMessage("Пароль має містити мінімум 8 символів.")
                .Matches(@"[A-Z]").WithMessage("Пароль має містити хоча б одну велику літеру.")
                .Matches(@"[0-9]").WithMessage("Пароль має містити хоча б одну цифру.");

            RuleFor(x => x.Dto.FirstName)
                .NotEmpty().WithMessage("Ім'я є обов'язковим.")
                .MaximumLength(100);

            RuleFor(x => x.Dto.LastName)
                .NotEmpty().WithMessage("Прізвище є обов'язковим.")
                .MaximumLength(100);

            RuleFor(x => x.Dto.PhoneNumber)
                .Matches(@"^\+?[0-9\s\-\(\)]{7,20}$")
                .When(x => x.Dto.PhoneNumber is not null)
                .WithMessage("Некоректний формат номера телефону.");

            RuleFor(x => x.Dto.Role)
                .Must(r => r is UserRole.Patient or UserRole.Doctor)
                .WithMessage("Реєстрація можлива лише як Пацієнт або Лікар.");
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
            var emailTaken = await unitOfWork.Users.AnyAsync(
                u => u.Email == request.Dto.Email.ToLowerInvariant(),
                cancellationToken);

            if (emailTaken)
                return Result<AuthResponseDto>.Failure($"Користувач з Email '{request.Dto.Email}' вже існує.");

            var user = new ApplicationUser
            {
                Id = Guid.NewGuid(),
                Email = request.Dto.Email.ToLowerInvariant(),
                FirstName = request.Dto.FirstName,
                LastName = request.Dto.LastName,
                PhoneNumber = request.Dto.PhoneNumber ?? string.Empty,
                Role = request.Dto.Role,
                IsActive = true,
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow,
            };

            user.PasswordHash = passwordHasher.HashPassword(user, request.Dto.Password);
            await unitOfWork.Users.AddAsync(user, cancellationToken);

            if (request.Dto.Role == UserRole.Doctor)
            {
                var doctorProfile = new DoctorProfile
                {
                    Id = Guid.NewGuid(),
                    UserId = user.Id,
                    CareerStartDate = DateTime.UtcNow,
                    Status = DoctorStatus.Active,
                    AverageRating = 0.0,
                    UpdatedAt = DateTime.UtcNow,
                };
                await unitOfWork.DoctorProfiles.AddAsync(doctorProfile, cancellationToken);
            }
            else if (request.Dto.Role == UserRole.Patient)
            {
                var patientProfile = new PatientProfile
                {
                    Id = Guid.NewGuid(),
                    UserId = user.Id,
                    Type = PatientType.Standard,
                    DateOfBirth = DateTime.UtcNow,
                };
                await unitOfWork.PatientProfiles.AddAsync(patientProfile, cancellationToken);
            }

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
                CreatedByIp = request.IpAddress
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