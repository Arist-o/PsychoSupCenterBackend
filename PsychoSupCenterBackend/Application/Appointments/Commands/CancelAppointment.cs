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

    public sealed class Handler(IUnitOfWork unitOfWork) : IRequestHandler<Command, Result<bool>>
    {
        public async Task<Result<bool>> Handle(Command request, CancellationToken cancellationToken)
        {
            var appointment = await unitOfWork.Appointments.GetByIdAsync(request.AppointmentId, cancellationToken);
            if (appointment is null) return Result<bool>.Failure("Запис не знайдено.");

            appointment.Status = AppointmentStatus.Cancelled;
            unitOfWork.Appointments.Update(appointment);

            return Result<bool>.Success(true);
        }
    }
}