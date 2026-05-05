using FluentValidation;
using MediatR;
using PsychoSupCenterBackend.Application.Billing.DTOs;
using PsychoSupCenterBackend.Application.Common.Behaviors;
using PsychoSupCenterBackend.Application.Common.Interfaces;
using PsychoSupCenterBackend.Application.Common.Models;
using PsychoSupCenterBackend.Domain.Enums;

namespace PsychoSupCenterBackend.Application.Billing.Commands;

public static class ProcessPayment
{
    public sealed record Command(
        Guid BillingId,
        PaymentMethod PaymentMethod
    ) : ICommand<Result<BillingResponseDto>>;

    public sealed class Validator : AbstractValidator<Command>
    {
        public Validator()
        {
            RuleFor(x => x.BillingId).NotEmpty();
            RuleFor(x => x.PaymentMethod).IsInEnum();
        }
    }

    public sealed class Handler(IUnitOfWork unitOfWork)
        : IRequestHandler<Command, Result<BillingResponseDto>>
    {
        public async Task<Result<BillingResponseDto>> Handle(
            Command request, CancellationToken cancellationToken)
        {
            var billing = await unitOfWork.Billings.GetByIdAsync(request.BillingId, cancellationToken);

            if (billing is null)
                return Result<BillingResponseDto>.Failure($"Білінг з Id '{request.BillingId}' не знайдено.");

            if (billing.PaymentStatus is PaymentStatus.Paid or PaymentStatus.Refunded)
                return Result<BillingResponseDto>.Failure("Рахунок вже оплачено або повернено.");

            billing.PaymentStatus = PaymentStatus.Paid;
            billing.PaidAt = DateTime.UtcNow;
            
           
            unitOfWork.Billings.Update(billing);
            await unitOfWork.SaveChangesAsync(cancellationToken);

            var service = await unitOfWork.DoctorServices.GetByIdAsync(billing.DoctorServiceId, cancellationToken);

            return Result<BillingResponseDto>.Success(new BillingResponseDto(
                billing.Id, 
                billing.DoctorServiceId, 
                service?.ServiceName ?? string.Empty,
                billing.Amount, 
                billing.PaymentStatus, 
                billing.CreatedAt, 
                billing.PaidAt));
        }
    }
}