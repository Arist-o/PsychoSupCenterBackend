
using FluentValidation;
using MediatR;
using PsychoSupCenterBackend.Application.Common.Behaviors;
using PsychoSupCenterBackend.Application.Common.Interfaces;
using PsychoSupCenterBackend.Application.Common.Models;
using PsychoSupCenterBackend.Application.DoctorCertificates.DTOs;
using PsychoSupCenterBackend.Domain.Entities;

namespace PsychoSupCenterBackend.Application.DoctorCertificates.Commands;

public static class AddDoctorCertificate
{
    public sealed record Command(AddDoctorCertificateDto Dto)
        : ICommand<Result<DoctorCertificateResponseDto>>;

    public sealed class Validator : AbstractValidator<Command>
    {
        public Validator()
        {
            RuleFor(x => x.Dto.DoctorProfileId).NotEmpty();
            RuleFor(x => x.Dto.CertificateUrl)
                .NotEmpty().MaximumLength(2048)
                .Must(url => Uri.TryCreate(url, UriKind.Absolute, out _))
                .WithMessage("Некоректний URL сертифіката.");
        }
    }

    public sealed class Handler(IUnitOfWork unitOfWork)
        : IRequestHandler<Command, Result<DoctorCertificateResponseDto>>
    {
        public async Task<Result<DoctorCertificateResponseDto>> Handle(
            Command request, CancellationToken cancellationToken)
        {
            var doctorExists = await unitOfWork.DoctorProfiles
                .AnyAsync(d => d.Id == request.Dto.DoctorProfileId, cancellationToken);

            if (!doctorExists)
                return Result<DoctorCertificateResponseDto>.Failure("Лікаря не знайдено.");

            var cert = new DoctorCertificate
            {
                Id = Guid.NewGuid(),
                DoctorProfileId = request.Dto.DoctorProfileId,
                CertificateUrl = request.Dto.CertificateUrl,
                AddedAt = DateTime.UtcNow,
            };

            await unitOfWork.DoctorCertificates.AddAsync(cert, cancellationToken);

            return Result<DoctorCertificateResponseDto>.Success(
                new DoctorCertificateResponseDto(
                    cert.Id, cert.DoctorProfileId, cert.CertificateUrl, cert.AddedAt));
        }
    }
}