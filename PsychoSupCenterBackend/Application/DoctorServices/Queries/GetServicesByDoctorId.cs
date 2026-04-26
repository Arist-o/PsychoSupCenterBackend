
using MediatR;
using PsychoSupCenterBackend.Application.Common.Behaviors;
using PsychoSupCenterBackend.Application.Common.Interfaces;
using PsychoSupCenterBackend.Application.Common.Models;
using PsychoSupCenterBackend.Application.DoctorServices.DTOs;

namespace PsychoSupCenterBackend.Application.DoctorServices.Queries;

public static class GetServicesByDoctorId
{
    public sealed record Query(Guid DoctorProfileId)
        : IQuery<Result<IReadOnlyList<DoctorServiceResponseDto>>>;

    public sealed class Handler(IUnitOfWork unitOfWork)
        : IRequestHandler<Query, Result<IReadOnlyList<DoctorServiceResponseDto>>>
    {
        public async Task<Result<IReadOnlyList<DoctorServiceResponseDto>>> Handle(
            Query request,
            CancellationToken cancellationToken)
        {
            var services = await unitOfWork.DoctorServices.FindAsync(
                s => s.DoctorProfileId == request.DoctorProfileId,
                cancellationToken);

            var result = services
                .Select(s => new DoctorServiceResponseDto(
                    s.Id, s.DoctorProfileId, s.ServiceName, s.Price))
                .ToList();

            return Result<IReadOnlyList<DoctorServiceResponseDto>>.Success(result);
        }
    }
}