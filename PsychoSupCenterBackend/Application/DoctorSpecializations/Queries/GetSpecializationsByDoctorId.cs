
using MediatR;
using Microsoft.EntityFrameworkCore;
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
            var doctor = await unitOfWork.DoctorProfiles.Query()
                .Include(d => d.Specializations)
                .FirstOrDefaultAsync(d => d.Id == request.DoctorProfileId, cancellationToken);

            if (doctor is null)
                return Result<IReadOnlyList<SpecializationResponseDto>>.Failure("Лікаря не знайдено.");

            var result = doctor.Specializations
                .OrderBy(s => s.Name)
                .Select(s => new SpecializationResponseDto(s.Id, s.Name, s.Description))
                .ToList();

            return Result<IReadOnlyList<SpecializationResponseDto>>.Success(result);
        }
    }
}