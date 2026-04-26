// Application/Billing/Commands/RefundBilling.cs
using FluentValidation;
using MediatR;
using PsychoSupCenterBackend.Application.Billing.DTOs;
using PsychoSupCenterBackend.Application.Common.Behaviors;
using PsychoSupCenterBackend.Application.Common.Interfaces;
using PsychoSupCenterBackend.Application.Common.Models;
using PsychoSupCenterBackend.Domain.Enums;

namespace PsychoSupCenterBackend.Application.Billing.Commands;

public static class RefundBilling
{
    public sealed record Command(Guid BillingId) : ICommand<Result<BillingResponseDto>>;

    public sealed class Validator : AbstractValidator<Command>
    {
        public Validator() => RuleFor(x => x.BillingId).NotEmpty();
    }

    public sealed class Handler(IUnitOfWork unitOfWork)
        : IRequestHandler<Command, Result<BillingResponseDto>>
    {
        public async Task<Result<BillingResponseDto>> Handle(
            Command request, CancellationToken cancellationToken)
        {
            var billing = await unitOfWork.Billings
                .GetByIdAsync(request.BillingId, cancellationToken);

            if (billing is null)
                return Result<BillingResponseDto>.Failure("Білінг не знайдено.");

            if (billing.PaymentStatus != PaymentStatus.Paid)
                return Result<BillingResponseDto>.Failure(
                    "Повернення коштів можливе лише для оплачених рахунків.");

            billing.PaymentStatus = PaymentStatus.Refunded;
            unitOfWork.Billings.Update(billing);

            var service = await unitOfWork.DoctorServices
                .GetByIdAsync(billing.DoctorServiceId, cancellationToken);

            return Result<BillingResponseDto>.Success(new BillingResponseDto(
                billing.Id, billing.DoctorServiceId, service?.ServiceName ?? string.Empty,
                billing.Amount, billing.PaymentStatus, billing.CreatedAt, billing.PaidAt));
        }
    }
}