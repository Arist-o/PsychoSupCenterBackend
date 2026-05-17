using FluentValidation;
using MediatR;
using PsychoSupCenterBackend.Application.Common.Behaviors;
using PsychoSupCenterBackend.Application.Common.Interfaces;
using PsychoSupCenterBackend.Application.Common.Models;
using PsychoSupCenterBackend.Domain.Enums;

namespace PsychoSupCenterBackend.Application.Appointments.Commands;

public static class CancelAppointment
{
    public sealed record Command(Guid AppointmentId) : ICommand<Result<bool>>;

    public sealed class Validator : AbstractValidator<Command>
    {
        public Validator() => RuleFor(x => x.AppointmentId).NotEmpty();
    }

    public sealed class Handler(IUnitOfWork unitOfWork, ICurrentUserAccessor currentUserAccessor) : IRequestHandler<Command, Result<bool>>
    {
        public async Task<Result<bool>> Handle(Command request, CancellationToken cancellationToken)
        {
            var appointment = await unitOfWork.Appointments.GetByIdAsync(request.AppointmentId, cancellationToken);
            if (appointment is null) return Result<bool>.Failure("Запис не знайдено.");

            var currentUserId = currentUserAccessor.UserId;
            var currentUserRole = currentUserAccessor.UserRole;

            if (currentUserRole == "Doctor")
            {
                var doctorProfile = await unitOfWork.DoctorProfiles.FirstOrDefaultAsync(d => d.UserId == Guid.Parse(currentUserId!), cancellationToken);
                if (doctorProfile == null || appointment.DoctorProfileId != doctorProfile.Id)
                {
                    return Result<bool>.Failure("Ви можете скасовувати лише власні зустрічі.");
                }
            }
            // Admin can cancel anything (no check needed here, but we can be explicit if we want)

            appointment.Status = AppointmentStatus.Cancelled;
            unitOfWork.Appointments.Update(appointment);
            await unitOfWork.SaveChangesAsync(cancellationToken);

            return Result<bool>.Success(true);
        }
    }
}