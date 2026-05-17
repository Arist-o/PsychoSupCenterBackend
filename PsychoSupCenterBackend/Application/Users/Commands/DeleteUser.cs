using FluentValidation;
using MediatR;
using PsychoSupCenterBackend.Application.Common.Behaviors;
using PsychoSupCenterBackend.Application.Common.Interfaces;
using PsychoSupCenterBackend.Application.Common.Models;

namespace PsychoSupCenterBackend.Application.Users.Commands;

public static class DeleteUser
{
    public sealed record Command(Guid UserId) : ICommand<Result<bool>>;

    public sealed class Validator : AbstractValidator<Command>
    {
        public Validator() => RuleFor(x => x.UserId).NotEmpty();
    }

    public sealed class Handler(IUnitOfWork unitOfWork) : IRequestHandler<Command, Result<bool>>
    {
        public async Task<Result<bool>> Handle(Command request, CancellationToken cancellationToken)
        {
            var user = await unitOfWork.Users.GetByIdAsync(request.UserId, cancellationToken);
            if (user is null) return Result<bool>.Failure("Користувача не знайдено.");

            var hasAppointments = await unitOfWork.Appointments.AnyAsync(
                a => (user.DoctorProfile != null && a.DoctorProfileId == user.DoctorProfile.Id) || 
                     (user.PatientProfile != null && a.PatientProfileId == user.PatientProfile.Id), 
                cancellationToken);

            if (hasAppointments)
                return Result<bool>.Failure("Неможливо видалити користувача, оскільки існують пов'язані записи на прийом.");

            user.IsActive = false;
            user.UpdatedAt = DateTime.UtcNow;

            unitOfWork.Users.Update(user);
            await unitOfWork.SaveChangesAsync(cancellationToken);
            
            return Result<bool>.Success(true);
        }
    }
}