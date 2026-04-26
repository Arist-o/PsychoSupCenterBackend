using FluentValidation;
using MediatR;
using PsychoSupCenterBackend.Application.Common.Behaviors;
using PsychoSupCenterBackend.Application.Common.Interfaces;
using PsychoSupCenterBackend.Application.Common.Models;
using PsychoSupCenterBackend.Application.Users.DTOs;

namespace PsychoSupCenterBackend.Application.Users.Commands;

public static class UpdateUser
{
    public sealed record Command(Guid UserId, UpdateUserDto Dto) : ICommand<Result<UserResponseDto>>;

    public sealed class Validator : AbstractValidator<Command>
    {
        public Validator()
        {
            RuleFor(x => x.UserId).NotEmpty();
            RuleFor(x => x.Dto.FirstName).NotEmpty().MaximumLength(100);
            RuleFor(x => x.Dto.LastName).NotEmpty().MaximumLength(100);
            RuleFor(x => x.Dto.PhoneNumber).Matches(@"^\+?[0-9\s\-\(\)]{7,20}$").When(x => x.Dto.PhoneNumber is not null);
        }
    }

    public sealed class Handler(IUnitOfWork unitOfWork) : IRequestHandler<Command, Result<UserResponseDto>>
    {
        public async Task<Result<UserResponseDto>> Handle(Command request, CancellationToken cancellationToken)
        {
            var user = await unitOfWork.Users.GetByIdAsync(request.UserId, cancellationToken);
            if (user is null) return Result<UserResponseDto>.Failure("Користувача не знайдено.");

            user.FirstName = request.Dto.FirstName;
            user.LastName = request.Dto.LastName;
            user.PhoneNumber = request.Dto.PhoneNumber ?? string.Empty;
            user.PhotoUrl = request.Dto.PhotoUrl;
            user.UpdatedAt = DateTime.UtcNow;

            unitOfWork.Users.Update(user);

            return Result<UserResponseDto>.Success(new UserResponseDto(
                user.Id, user.Email, user.FirstName, user.LastName, user.PhoneNumber, user.PhotoUrl, user.Role.ToString(), user.IsActive));
        }
    }
}