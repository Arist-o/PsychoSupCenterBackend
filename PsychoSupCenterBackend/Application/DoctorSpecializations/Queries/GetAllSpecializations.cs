
using MediatR;
using Microsoft.EntityFrameworkCore;
using PsychoSupCenterBackend.Application.Common.Behaviors;
using PsychoSupCenterBackend.Application.Common.Interfaces;
using PsychoSupCenterBackend.Application.Common.Models;
using PsychoSupCenterBackend.Application.DoctorSpecializations.DTOs;

namespace PsychoSupCenterBackend.Application.DoctorSpecializations.Queries;

public static class GetAllSpecializations
{
    public sealed record Query(int Page = 1, int PageSize = 50)
        : IQuery<Result<IReadOnlyList<SpecializationResponseDto>>>;

    public sealed class Handler(IUnitOfWork unitOfWork)
        : IRequestHandler<Query, Result<IReadOnlyList<SpecializationResponseDto>>>
    {
        public async Task<Result<IReadOnlyList<SpecializationResponseDto>>> Handle(
            Query request, CancellationToken cancellationToken)
        {
            var query = unitOfWork.DoctorSpecializations.Query()
                .Include(s => s.DoctorProfiles)
                .OrderBy(s => s.Name)
                .Skip((request.Page - 1) * request.PageSize)
                .Take(request.PageSize);

            var paged = await query
                .Select(s => new SpecializationResponseDto(s.Id, s.Name, s.Description, s.DoctorProfiles.Select(p => p.Id)))
                .ToListAsync(cancellationToken);

            return Result<IReadOnlyList<SpecializationResponseDto>>.Success(paged);
        }
    }
}