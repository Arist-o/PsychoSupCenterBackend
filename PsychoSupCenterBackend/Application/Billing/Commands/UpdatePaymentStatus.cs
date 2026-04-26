// Application/Billing/Commands/UpdatePaymentStatus.cs
using FluentValidation;
using MediatR;
using PsychoSupCenterBackend.Application.Billing.DTOs;
using PsychoSupCenterBackend.Application.Common.Behaviors;
using PsychoSupCenterBackend.Application.Common.Interfaces;
using PsychoSupCenterBackend.Application.Common.Models;
using PsychoSupCenterBackend.Domain.Enums;

namespace PsychoSupCenterBackend.Application.Billing.Commands;

public static class UpdatePaymentStatus
{
    public sealed record Command(
        Guid BillingId,
        PaymentStatus NewStatus
    ) : ICommand<Result<BillingResponseDto>>;

    public sealed class Validator : AbstractValidator<Command>
    {
        public Validator()
        {
            RuleFor(x => x.BillingId).NotEmpty();
            RuleFor(x => x.NewStatus).IsInEnum();
        }
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
                return Result<BillingResponseDto>.Failure(
                    $"Білінг з Id '{request.BillingId}' не знайдено.");

            if (billing.PaymentStatus == PaymentStatus.Paid
                && request.NewStatus == PaymentStatus.Pending)
                return Result<BillingResponseDto>.Failure(
                    "Неможливо змінити статус оплаченого рахунку на Pending.");

            billing.PaymentStatus = request.NewStatus;

            if (request.NewStatus == PaymentStatus.Paid)
                billing.PaidAt = DateTime.UtcNow;

            unitOfWork.Billings.Update(billing);

            var service = await unitOfWork.DoctorServices
                .GetByIdAsync(billing.DoctorServiceId, cancellationToken);

            return Result<BillingResponseDto>.Success(new BillingResponseDto(
                billing.Id, billing.DoctorServiceId, service?.ServiceName ?? string.Empty,
                billing.Amount, billing.PaymentStatus, billing.CreatedAt, billing.PaidAt));
        }
    }
}