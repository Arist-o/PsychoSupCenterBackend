
using MediatR;
using Microsoft.EntityFrameworkCore;
using PsychoSupCenterBackend.Application.Common.Behaviors;
using PsychoSupCenterBackend.Application.Common.Interfaces;
using PsychoSupCenterBackend.Application.Common.Models;
using PsychoSupCenterBackend.Application.Doctors.DTOs;
using PsychoSupCenterBackend.Domain.Enums;

namespace PsychoSupCenterBackend.Application.Doctors.Queries;

public static class GetAllDoctors
{
    public sealed record Query(
        DoctorStatus? StatusFilter = null,
        int Page = 1,
        int PageSize = 20
    ) : IQuery<Result<IReadOnlyList<DoctorProfileResponseDto>>>;

    public sealed class Handler(IUnitOfWork unitOfWork)
        : IRequestHandler<Query, Result<IReadOnlyList<DoctorProfileResponseDto>>>
    {
        public async Task<Result<IReadOnlyList<DoctorProfileResponseDto>>> Handle(
            Query request,
            CancellationToken cancellationToken)
        {
            var query = unitOfWork.DoctorProfiles
                .Query()
                .Include(d => d.User)
                .Where(d => d.User.IsActive);

            if (request.StatusFilter.HasValue)
                query = query.Where(d => d.Status == request.StatusFilter.Value);

            var doctors = await query
                .OrderByDescending(d => d.AverageRating)
                .Skip((request.Page - 1) * request.PageSize)
                .Take(request.PageSize)
                .Select(d => new DoctorProfileResponseDto(
                    d.Id,
                    d.UserId,
                    d.User.FirstName,
                    d.User.LastName,
                    d.User.Email,
                    d.User.PhotoUrl,
                    d.Bio,
                    d.CareerStartDate,
                    (int)((DateTime.UtcNow - d.CareerStartDate).TotalDays / 365.25),
                    d.Status,
                    d.AverageRating,
                    d.UpdatedAt))
                .ToListAsync(cancellationToken);

            return Result<IReadOnlyList<DoctorProfileResponseDto>>.Success(doctors);
        }
    }
}