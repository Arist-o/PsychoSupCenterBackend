using FluentValidation;
using MediatR;
using Microsoft.AspNetCore.Http;
using PsychoSupCenterBackend.Application.Common.Behaviors;
using PsychoSupCenterBackend.Application.Common.Interfaces;
using PsychoSupCenterBackend.Application.Common.Models;
using PsychoSupCenterBackend.Application.DoctorCertificates.DTOs;
using PsychoSupCenterBackend.Domain.Entities;

namespace PsychoSupCenterBackend.Application.DoctorCertificates.Commands;

public static class UploadDoctorCertificate
{
    public sealed record Command(Guid DoctorProfileId, IFormFile File)
        : ICommand<Result<DoctorCertificateResponseDto>>;

    public sealed class Validator : AbstractValidator<Command>
    {
        public Validator()
        {
            RuleFor(x => x.DoctorProfileId).NotEmpty();
            RuleFor(x => x.File).NotNull().WithMessage("Файл не обрано.");
            RuleFor(x => x.File.Length).LessThanOrEqualTo(5 * 1024 * 1024).WithMessage("Розмір файлу не повинен перевищувати 5МБ.");
            RuleFor(x => x.File.ContentType).Must(x => x.StartsWith("image/") || x == "application/pdf")
                .WithMessage("Файл повинен бути зображенням або PDF.");
        }
    }

    public sealed class Handler(IUnitOfWork unitOfWork, IPhotoService photoService)
        : IRequestHandler<Command, Result<DoctorCertificateResponseDto>>
    {
        public async Task<Result<DoctorCertificateResponseDto>> Handle(
            Command request, CancellationToken cancellationToken)
        {
            var doctorExists = await unitOfWork.DoctorProfiles
                .AnyAsync(d => d.Id == request.DoctorProfileId, cancellationToken);

            if (!doctorExists)
                return Result<DoctorCertificateResponseDto>.Failure("Лікаря не знайдено.");

            var uploadResult = await photoService.UploadPhotoAsync(request.File);
            if (uploadResult is null)
                return Result<DoctorCertificateResponseDto>.Failure("Помилка завантаження файлу.");

            var cert = new DoctorCertificate
            {
                Id = Guid.NewGuid(),
                DoctorProfileId = request.DoctorProfileId,
                CertificateUrl = uploadResult.Url,
                AddedAt = DateTime.UtcNow,
            };

            await unitOfWork.DoctorCertificates.AddAsync(cert, cancellationToken);

            return Result<DoctorCertificateResponseDto>.Success(
                new DoctorCertificateResponseDto(
                    cert.Id, cert.DoctorProfileId, cert.CertificateUrl, cert.AddedAt));
        }
    }
}