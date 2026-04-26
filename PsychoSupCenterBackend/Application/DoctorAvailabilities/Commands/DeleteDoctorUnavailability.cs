using FluentValidation;
using MediatR;
using PsychoSupCenterBackend.Application.Common.Behaviors;
using PsychoSupCenterBackend.Application.Common.Interfaces;
using PsychoSupCenterBackend.Application.Common.Models;

namespace PsychoSupCenterBackend.Application.DoctorAvailabilities.Commands;

public static class DeleteDoctorUnavailability
{
    public sealed record Command(Guid UnavailabilityId) : ICommand<Result<bool>>;

    public sealed class Validator : AbstractValidator<Command>
    {
        public Validator() => RuleFor(x => x.UnavailabilityId).NotEmpty();
    }

    public sealed class Handler(IUnitOfWork unitOfWork) : IRequestHandler<Command, Result<bool>>
    {
        public async Task<Result<bool>> Handle(Command request, CancellationToken cancellationToken)
        {
            var unavailability = await unitOfWork.DoctorUnavailabilities.GetByIdAsync(request.UnavailabilityId, cancellationToken);
            if (unavailability is null) return Result<bool>.Failure("Запис про відсутність не знайдено.");

            unitOfWork.DoctorUnavailabilities.Remove(unavailability);
            return Result<bool>.Success(true);
        }
    }
}