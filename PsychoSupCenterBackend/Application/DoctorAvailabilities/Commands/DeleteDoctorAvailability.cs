
using FluentValidation;
using MediatR;
using PsychoSupCenterBackend.Application.Common.Behaviors;
using PsychoSupCenterBackend.Application.Common.Interfaces;
using PsychoSupCenterBackend.Application.Common.Models;

namespace PsychoSupCenterBackend.Application.DoctorAvailabilities.Commands;

public static class DeleteDoctorAvailability
{
    public sealed record Command(Guid AvailabilityId) : ICommand<Result<bool>>;

    public sealed class Validator : AbstractValidator<Command>
    {
        public Validator() => RuleFor(x => x.AvailabilityId).NotEmpty();
    }

    public sealed class Handler(IUnitOfWork unitOfWork)
        : IRequestHandler<Command, Result<bool>>
    {
        public async Task<Result<bool>> Handle(
            Command request, CancellationToken cancellationToken)
        {
            var availability = await unitOfWork.DoctorAvailabilities
                .GetByIdAsync(request.AvailabilityId, cancellationToken);

            if (availability is null)
                return Result<bool>.Failure("Слот не знайдено.");

            unitOfWork.DoctorAvailabilities.Remove(availability);
            return Result<bool>.Success(true);
        }
    }
}