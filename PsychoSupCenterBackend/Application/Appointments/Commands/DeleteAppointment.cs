using FluentValidation;
using MediatR;
using PsychoSupCenterBackend.Application.Common.Behaviors;
using PsychoSupCenterBackend.Application.Common.Interfaces;
using PsychoSupCenterBackend.Application.Common.Models;

namespace PsychoSupCenterBackend.Application.Appointments.Commands;

public static class DeleteAppointment
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

            if (appointment.BillingId.HasValue)
            {
                var billing = await unitOfWork.Billings.GetByIdAsync(appointment.BillingId.Value, cancellationToken);
                if (billing is not null) unitOfWork.Billings.Remove(billing);
            }

            if (appointment.ChatRoomId.HasValue)
            {
                var chatRoom = await unitOfWork.ChatRooms.GetByIdAsync(appointment.ChatRoomId.Value, cancellationToken);
                if (chatRoom is not null) unitOfWork.ChatRooms.Remove(chatRoom);
            }

            unitOfWork.Appointments.Remove(appointment);
            return Result<bool>.Success(true);
        }
    }
}