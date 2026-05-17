using FluentValidation;
using MediatR;
using Microsoft.AspNetCore.Identity;
using PsychoSupCenterBackend.Application.Admin.DTOs;
using PsychoSupCenterBackend.Application.Common.Behaviors;
using PsychoSupCenterBackend.Application.Common.Interfaces;
using PsychoSupCenterBackend.Application.Common.Models;
using PsychoSupCenterBackend.Application.Users.DTOs;
using PsychoSupCenterBackend.Domain.Entities;
using PsychoSupCenterBackend.Domain.Enums;

namespace PsychoSupCenterBackend.Application.Admin.Commands;

public static class CreateAdminUser
{
    public sealed record Command(CreateAdminDto Dto) : ICommand<Result<UserResponseDto>>;

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
        }
    }

    public sealed class Handler(
        IUnitOfWork unitOfWork,
        IPasswordHasher<ApplicationUser> passwordHasher)
        : IRequestHandler<Command, Result<UserResponseDto>>
    {
        public async Task<Result<UserResponseDto>> Handle(
            Command request, CancellationToken cancellationToken)
        {
            var emailTaken = await unitOfWork.Users.AnyAsync(
                u => u.Email == request.Dto.Email.ToLowerInvariant(),
                cancellationToken);

            if (emailTaken)
                return Result<UserResponseDto>.Failure($"Користувач з Email '{request.Dto.Email}' вже існує.");

            var user = new ApplicationUser
            {
                Id = Guid.NewGuid(),
                Email = request.Dto.Email.ToLowerInvariant(),
                FirstName = request.Dto.FirstName,
                LastName = request.Dto.LastName,
                Age = request.Dto.Age,
                Role = UserRole.Admin,
                IsActive = true,
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow,
            };

            user.PasswordHash = passwordHasher.HashPassword(user, request.Dto.Password);
            
            await unitOfWork.Users.AddAsync(user, cancellationToken);
            await unitOfWork.SaveChangesAsync(cancellationToken);

            return Result<UserResponseDto>.Success(new UserResponseDto(
                user.Id,
                user.Email,
                user.FirstName,
                user.LastName,
                user.Age,
                user.PhoneNumber ?? string.Empty,
                user.PhotoUrl,
                user.Role.ToString(),
                user.IsActive
            ));
        }
    }
}