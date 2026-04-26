
using MediatR;
using PsychoSupCenterBackend.Application.Common.Behaviors;
using PsychoSupCenterBackend.Application.Common.Interfaces;
using PsychoSupCenterBackend.Application.Common.Models;
using PsychoSupCenterBackend.Application.DoctorSpecializations.DTOs;

namespace PsychoSupCenterBackend.Application.DoctorSpecializations.Queries;

public static class GetSpecializationsByDoctorId
{
    public sealed record Query(Guid DoctorProfileId)
        : IQuery<Result<IReadOnlyList<SpecializationResponseDto>>>;

    public sealed class Handler(IUnitOfWork unitOfWork)
        : IRequestHandler<Query, Result<IReadOnlyList<SpecializationResponseDto>>>
    {
        public async Task<Result<IReadOnlyList<SpecializationResponseDto>>> Handle(
            Query request, CancellationToken cancellationToken)
        {
            var specs = await unitOfWork.DoctorSpecializations.FindAsync(
                s => s.DoctorProfileId == request.DoctorProfileId,
                cancellationToken);

            var result = specs
                .OrderBy(s => s.Name)
                .Select(s => new SpecializationResponseDto(s.Id, s.DoctorProfileId, s.Name))
                .ToList();

            return Result<IReadOnlyList<SpecializationResponseDto>>.Success(result);
        }
    }
}