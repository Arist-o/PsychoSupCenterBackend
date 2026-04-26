using FluentValidation;
using MediatR;
using PsychoSupCenterBackend.Application.Common.Behaviors;
using PsychoSupCenterBackend.Application.Common.Interfaces;
using PsychoSupCenterBackend.Application.Common.Models;
using PsychoSupCenterBackend.Application.Users.DTOs;

namespace PsychoSupCenterBackend.Application.Users.Queries;

public static class GetUserById
{
    public sealed record Query(Guid UserId) : IQuery<Result<UserResponseDto>>;

    public sealed class Validator : AbstractValidator<Query>
    {
        public Validator() => RuleFor(x => x.UserId).NotEmpty();
    }

    public sealed class Handler(IUnitOfWork unitOfWork) : IRequestHandler<Query, Result<UserResponseDto>>
    {
        public async Task<Result<UserResponseDto>> Handle(Query request, CancellationToken cancellationToken)
        {
            var user = await unitOfWork.Users.GetByIdAsync(request.UserId, cancellationToken);

            if (user is null)
                return Result<UserResponseDto>.Failure("Користувача не знайдено.");

            return Result<UserResponseDto>.Success(new UserResponseDto(
                user.Id, user.Email, user.FirstName, user.LastName, user.PhoneNumber!, user.PhotoUrl, user.Role.ToString(), user.IsActive));
        }
    }
}