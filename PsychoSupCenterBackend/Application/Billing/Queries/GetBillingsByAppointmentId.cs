using FluentValidation;
using MediatR;
using Microsoft.EntityFrameworkCore;
using PsychoSupCenterBackend.Application.Billing.DTOs;
using PsychoSupCenterBackend.Application.Common.Behaviors;
using PsychoSupCenterBackend.Application.Common.Interfaces;
using PsychoSupCenterBackend.Application.Common.Models;

namespace PsychoSupCenterBackend.Application.Billing.Queries;

public static class GetBillingsByAppointmentId
{
    public sealed record Query(Guid AppointmentId) : IQuery<Result<BillingResponseDto?>>;

    public sealed class Validator : AbstractValidator<Query>
    {
        public Validator() => RuleFor(x => x.AppointmentId).NotEmpty();
    }

    public sealed class Handler(IUnitOfWork unitOfWork)
        : IRequestHandler<Query, Result<BillingResponseDto?>>
    {
        public async Task<Result<BillingResponseDto?>> Handle(
            Query request, CancellationToken cancellationToken)
        {
            var appointment = await unitOfWork.Appointments
                .GetByIdAsync(request.AppointmentId, cancellationToken);

            if (appointment is null)
                return Result<BillingResponseDto?>.Failure("Запис не знайдено.");

            if (!appointment.BillingId.HasValue)
                return Result<BillingResponseDto?>.Success(null);

            var billing = await unitOfWork.Billings
                .Query()
                .Include(b => b.DoctorService)
                .FirstOrDefaultAsync(b => b.Id == appointment.BillingId.Value, cancellationToken);

            if (billing is null)
                return Result<BillingResponseDto?>.Success(null);

            return Result<BillingResponseDto?>.Success(new BillingResponseDto(
                billing.Id, billing.DoctorServiceId, billing.DoctorService.ServiceName,
                billing.Amount, billing.PaymentStatus, billing.CreatedAt, billing.PaidAt));
        }
    }
}