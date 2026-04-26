
using FluentValidation;
using MediatR;
using PsychoSupCenterBackend.Application.Common.Behaviors;
using PsychoSupCenterBackend.Application.Common.Interfaces;
using PsychoSupCenterBackend.Application.Common.Models;

namespace PsychoSupCenterBackend.Application.DoctorServices.Commands;

public static class DeleteDoctorService
{
    public sealed record Command(Guid ServiceId) : ICommand<Result<bool>>;

    public sealed class Validator : AbstractValidator<Command>
    {
        public Validator()
        {
            RuleFor(x => x.ServiceId).NotEmpty();
        }
    }

    public sealed class Handler(IUnitOfWork unitOfWork)
        : IRequestHandler<Command, Result<bool>>
    {
        public async Task<Result<bool>> Handle(
            Command request,
            CancellationToken cancellationToken)
        {
            var service = await unitOfWork.DoctorServices
                .GetByIdAsync(request.ServiceId, cancellationToken);

            if (service is null)
                return Result<bool>.Failure(
                    $"Послугу з Id '{request.ServiceId}' не знайдено.");

            var hasAppointments = await unitOfWork.Appointments
                .AnyAsync(a => a.DoctorServiceId == request.ServiceId, cancellationToken);

            if (hasAppointments)
                return Result<bool>.Failure(
                    "Неможливо видалити послугу, що має пов'язані записи.");

            unitOfWork.DoctorServices.Remove(service);

            return Result<bool>.Success(true);
        }
    }
}