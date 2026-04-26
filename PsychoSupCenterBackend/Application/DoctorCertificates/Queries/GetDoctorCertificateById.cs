
using FluentValidation;
using MediatR;
using PsychoSupCenterBackend.Application.Common.Behaviors;
using PsychoSupCenterBackend.Application.Common.Interfaces;
using PsychoSupCenterBackend.Application.Common.Models;
using PsychoSupCenterBackend.Application.DoctorCertificates.DTOs;

namespace PsychoSupCenterBackend.Application.DoctorCertificates.Queries;

public static class GetDoctorCertificateById
{
    public sealed record Query(Guid CertificateId)
        : IQuery<Result<DoctorCertificateResponseDto>>;

    public sealed class Validator : AbstractValidator<Query>
    {
        public Validator() => RuleFor(x => x.CertificateId).NotEmpty();
    }

    public sealed class Handler(IUnitOfWork unitOfWork)
        : IRequestHandler<Query, Result<DoctorCertificateResponseDto>>
    {
        public async Task<Result<DoctorCertificateResponseDto>> Handle(
            Query request, CancellationToken cancellationToken)
        {
            var cert = await unitOfWork.DoctorCertificates
                .GetByIdAsync(request.CertificateId, cancellationToken);

            if (cert is null)
                return Result<DoctorCertificateResponseDto>.Failure("Сертифікат не знайдено.");

            return Result<DoctorCertificateResponseDto>.Success(
                new DoctorCertificateResponseDto(
                    cert.Id, cert.DoctorProfileId, cert.CertificateUrl, cert.AddedAt));
        }
    }
}