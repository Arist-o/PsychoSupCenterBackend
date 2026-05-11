
using Application.DoctorCertificates.DTOs;
using FluentValidation;
using MediatR;
using PsychoSupCenterBackend.Application.Common.Behaviors;
using PsychoSupCenterBackend.Application.Common.Interfaces;
using PsychoSupCenterBackend.Application.Common.Models;
using PsychoSupCenterBackend.Application.DoctorCertificates.DTOs;

namespace PsychoSupCenterBackend.Application.DoctorCertificates.Commands;

public static class UpdateDoctorCertificate
{
    public sealed record Command(Guid CertificateId, UpdateDoctorCertificateDto Dto) : ICommand<Result<DoctorCertificateResponseDto>>;

    public sealed class Validator : AbstractValidator<Command>
    {
        public Validator()
        {
            RuleFor(x => x.CertificateId).NotEmpty();
            RuleFor(x => x.Dto.Name).NotEmpty().MaximumLength(200);
            RuleFor(x => x.Dto.IssuingOrganization).NotEmpty().MaximumLength(200);
            RuleFor(x => x.Dto.IssueDate).NotEmpty();
            RuleFor(x => x.Dto.CertificateUrl)
                .NotEmpty().MaximumLength(2048)
                .Must(url => Uri.TryCreate(url, UriKind.Absolute, out _))
                .WithMessage("Некоректний URL.");
        }
    }

    public sealed class Handler(IUnitOfWork unitOfWork)
        : IRequestHandler<Command, Result<DoctorCertificateResponseDto>>
    {
        public async Task<Result<DoctorCertificateResponseDto>> Handle(
            Command request, CancellationToken cancellationToken)
        {
            var cert = await unitOfWork.DoctorCertificates
                .GetByIdAsync(request.CertificateId, cancellationToken);

            if (cert is null)
                return Result<DoctorCertificateResponseDto>.Failure("Сертифікат не знайдено.");

            cert.Name = request.Dto.Name;
            cert.IssuingOrganization = request.Dto.IssuingOrganization;
            cert.IssueDate = request.Dto.IssueDate;
            cert.CertificateUrl = request.Dto.CertificateUrl;

            unitOfWork.DoctorCertificates.Update(cert);

            return Result<DoctorCertificateResponseDto>.Success(
                new DoctorCertificateResponseDto(
                    cert.Id, 
                    cert.DoctorProfileId, 
                    cert.Name,
                    cert.IssuingOrganization,
                    cert.IssueDate,
                    cert.CertificateUrl, 
                    cert.AddedAt));
        }
    }
}