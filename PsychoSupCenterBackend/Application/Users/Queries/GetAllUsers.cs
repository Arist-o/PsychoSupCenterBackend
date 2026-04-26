using MediatR;
using Microsoft.EntityFrameworkCore;
using PsychoSupCenterBackend.Application.Common.Behaviors;
using PsychoSupCenterBackend.Application.Common.Interfaces;
using PsychoSupCenterBackend.Application.Common.Models;
using PsychoSupCenterBackend.Application.Users.DTOs;

namespace PsychoSupCenterBackend.Application.Users.Queries;

public static class GetAllUsers
{
    public sealed record Query(int Page = 1, int PageSize = 20) : IQuery<Result<IReadOnlyList<UserResponseDto>>>;

    public sealed class Handler(IUnitOfWork unitOfWork) : IRequestHandler<Query, Result<IReadOnlyList<UserResponseDto>>>
    {
        public async Task<Result<IReadOnlyList<UserResponseDto>>> Handle(Query request, CancellationToken cancellationToken)
        {
            var users = await unitOfWork.Users.Query()
                .OrderByDescending(u => u.CreatedAt)
                .Skip((request.Page - 1) * request.PageSize)
                .Take(request.PageSize)
                .Select(u => new UserResponseDto(
                    u.Id, u.Email, u.FirstName, u.LastName, u.PhoneNumber!, u.PhotoUrl, u.Role.ToString(), u.IsActive))
                .ToListAsync(cancellationToken);

            return Result<IReadOnlyList<UserResponseDto>>.Success(users);
        }
    }
}