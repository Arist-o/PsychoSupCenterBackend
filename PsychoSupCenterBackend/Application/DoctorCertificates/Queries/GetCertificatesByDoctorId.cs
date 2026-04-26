
using MediatR;
using PsychoSupCenterBackend.Application.Common.Behaviors;
using PsychoSupCenterBackend.Application.Common.Interfaces;
using PsychoSupCenterBackend.Application.Common.Models;
using PsychoSupCenterBackend.Application.DoctorCertificates.DTOs;

namespace PsychoSupCenterBackend.Application.DoctorCertificates.Queries;

public static class GetCertificatesByDoctorId
{
    public sealed record Query(Guid DoctorProfileId)
        : IQuery<Result<IReadOnlyList<DoctorCertificateResponseDto>>>;

    public sealed class Handler(IUnitOfWork unitOfWork)
        : IRequestHandler<Query, Result<IReadOnlyList<DoctorCertificateResponseDto>>>
    {
        public async Task<Result<IReadOnlyList<DoctorCertificateResponseDto>>> Handle(
            Query request, CancellationToken cancellationToken)
        {
            var certs = await unitOfWork.DoctorCertificates.FindAsync(
                c => c.DoctorProfileId == request.DoctorProfileId, cancellationToken);

            var result = certs
                .OrderByDescending(c => c.AddedAt)
                .Select(c => new DoctorCertificateResponseDto(
                    c.Id, c.DoctorProfileId, c.CertificateUrl, c.AddedAt))
                .ToList();

            return Result<IReadOnlyList<DoctorCertificateResponseDto>>.Success(result);
        }
    }
}