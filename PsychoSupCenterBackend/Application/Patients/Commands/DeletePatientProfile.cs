using FluentValidation;
using MediatR;
using PsychoSupCenterBackend.Application.Common.Behaviors;
using PsychoSupCenterBackend.Application.Common.Interfaces;
using PsychoSupCenterBackend.Application.Common.Models;

namespace PsychoSupCenterBackend.Application.Patients.Commands;

public static class DeletePatientProfile
{
    public sealed record Command(Guid PatientProfileId) : ICommand<Result<bool>>;

    public sealed class Validator : AbstractValidator<Command>
    {
        public Validator() => RuleFor(x => x.PatientProfileId).NotEmpty();
    }

    public sealed class Handler(IUnitOfWork unitOfWork)
        : IRequestHandler<Command, Result<bool>>
    {
        public async Task<Result<bool>> Handle(
            Command request, CancellationToken cancellationToken)
        {
            var patient = await unitOfWork.PatientProfiles.GetByIdAsync(request.PatientProfileId, cancellationToken);

            if (patient is null)
                return Result<bool>.Failure($"Профіль пацієнта з Id '{request.PatientProfileId}' не знайдено.");

            var hasAppointments = await unitOfWork.Appointments.AnyAsync(a => a.PatientProfileId == request.PatientProfileId, cancellationToken);
            if (hasAppointments)
                return Result<bool>.Failure("Неможливо видалити профіль, оскільки існують пов'язані записи.");

            unitOfWork.PatientProfiles.Remove(patient);
            return Result<bool>.Success(true);
        }
    }
}