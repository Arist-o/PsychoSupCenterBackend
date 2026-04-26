using FluentValidation;
using MediatR;
using PsychoSupCenterBackend.Application.Common.Behaviors;
using PsychoSupCenterBackend.Application.Common.Interfaces;
using PsychoSupCenterBackend.Application.Common.Models;
using PsychoSupCenterBackend.Domain.Enums;

namespace PsychoSupCenterBackend.Application.Doctors.Commands;

public static class DeleteDoctorProfile
{
    public sealed record Command(Guid DoctorProfileId) : ICommand<Result<bool>>;

    public sealed class Validator : AbstractValidator<Command>
    {
        public Validator() => RuleFor(x => x.DoctorProfileId).NotEmpty();
    }

    public sealed class Handler(IUnitOfWork unitOfWork)
        : IRequestHandler<Command, Result<bool>>
    {
        public async Task<Result<bool>> Handle(
            Command request, CancellationToken cancellationToken)
        {
            var profile = await unitOfWork.DoctorProfiles
                .GetByIdAsync(request.DoctorProfileId, cancellationToken);

            if (profile is null)
                return Result<bool>.Failure("Профіль лікаря не знайдено.");

            var hasAppointments = await unitOfWork.Appointments.AnyAsync(
                a => a.DoctorProfileId == request.DoctorProfileId
                  && a.Status == AppointmentStatus.Scheduled,
                cancellationToken);

            if (hasAppointments)
                return Result<bool>.Failure(
                    "Неможливо видалити профіль лікаря з активними записами.");

            unitOfWork.DoctorProfiles.Remove(profile);

            return Result<bool>.Success(true);
        }
    }
}