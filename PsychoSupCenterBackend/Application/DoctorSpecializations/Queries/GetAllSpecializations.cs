
using MediatR;
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
            var all = await unitOfWork.DoctorSpecializations.GetAllAsync(cancellationToken);

            var paged = all
                .OrderBy(s => s.Name)
                .Skip((request.Page - 1) * request.PageSize)
                .Take(request.PageSize)
                .Select(s => new SpecializationResponseDto(s.Id, s.DoctorProfileId, s.Name))
                .ToList();

            return Result<IReadOnlyList<SpecializationResponseDto>>.Success(paged);
        }
    }
}