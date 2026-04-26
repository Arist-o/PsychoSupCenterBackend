
using FluentValidation;
using MediatR;
using PsychoSupCenterBackend.Application.Common.Behaviors;
using PsychoSupCenterBackend.Application.Common.Interfaces;
using PsychoSupCenterBackend.Application.Common.Models;

namespace PsychoSupCenterBackend.Application.DoctorCertificates.Commands;

public static class RemoveDoctorCertificate
{
    public sealed record Command(Guid CertificateId) : ICommand<Result<bool>>;

    public sealed class Validator : AbstractValidator<Command>
    {
        public Validator() => RuleFor(x => x.CertificateId).NotEmpty();
    }

    public sealed class Handler(IUnitOfWork unitOfWork)
        : IRequestHandler<Command, Result<bool>>
    {
        public async Task<Result<bool>> Handle(
            Command request, CancellationToken cancellationToken)
        {
            var cert = await unitOfWork.DoctorCertificates
                .GetByIdAsync(request.CertificateId, cancellationToken);

            if (cert is null)
                return Result<bool>.Failure("Сертифікат не знайдено.");

            unitOfWork.DoctorCertificates.Remove(cert);
            return Result<bool>.Success(true);
        }
    }
}