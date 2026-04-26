using MediatR;
using Microsoft.EntityFrameworkCore;
using PsychoSupCenterBackend.Application.Common.Behaviors;
using PsychoSupCenterBackend.Application.Common.Interfaces;
using PsychoSupCenterBackend.Application.Common.Models;
using PsychoSupCenterBackend.Application.DoctorServices.DTOs;

namespace PsychoSupCenterBackend.Application.DoctorServices.Queries;

public static class GetAllActiveServices
{
    public sealed record Query(int Page = 1, int PageSize = 50)
        : IQuery<Result<IReadOnlyList<DoctorServiceResponseDto>>>;

    public sealed class Handler(IUnitOfWork unitOfWork)
        : IRequestHandler<Query, Result<IReadOnlyList<DoctorServiceResponseDto>>>
    {
        public async Task<Result<IReadOnlyList<DoctorServiceResponseDto>>> Handle(
            Query request, CancellationToken cancellationToken)
        {
            var services = await unitOfWork.DoctorServices
                .Query()
                .Include(s => s.DoctorProfile)
                .Where(s => s.DoctorProfile.Status == Domain.Enums.DoctorStatus.Active)
                .OrderBy(s => s.ServiceName)
                .Skip((request.Page - 1) * request.PageSize)
                .Take(request.PageSize)
                .Select(s => new DoctorServiceResponseDto(
                    s.Id, s.DoctorProfileId, s.ServiceName, s.Price))
                .ToListAsync(cancellationToken);

            return Result<IReadOnlyList<DoctorServiceResponseDto>>.Success(services);
        }
    }
}