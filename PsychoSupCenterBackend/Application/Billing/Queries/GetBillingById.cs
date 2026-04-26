// Application/Billing/Queries/GetBillingById.cs
using FluentValidation;
using MediatR;
using Microsoft.EntityFrameworkCore;
using PsychoSupCenterBackend.Application.Billing.DTOs;
using PsychoSupCenterBackend.Application.Common.Behaviors;
using PsychoSupCenterBackend.Application.Common.Interfaces;
using PsychoSupCenterBackend.Application.Common.Models;

namespace PsychoSupCenterBackend.Application.Billing.Queries;

public static class GetBillingById
{
    public sealed record Query(Guid BillingId) : IQuery<Result<BillingResponseDto>>;

    public sealed class Validator : AbstractValidator<Query>
    {
        public Validator() => RuleFor(x => x.BillingId).NotEmpty();
    }

    public sealed class Handler(IUnitOfWork unitOfWork)
        : IRequestHandler<Query, Result<BillingResponseDto>>
    {
        public async Task<Result<BillingResponseDto>> Handle(
            Query request, CancellationToken cancellationToken)
        {
            var billing = await unitOfWork.Billings
                .Query()
                .Include(b => b.DoctorService)
                .FirstOrDefaultAsync(b => b.Id == request.BillingId, cancellationToken);

            if (billing is null)
                return Result<BillingResponseDto>.Failure("Білінг не знайдено.");

            return Result<BillingResponseDto>.Success(new BillingResponseDto(
                billing.Id, billing.DoctorServiceId, billing.DoctorService.ServiceName,
                billing.Amount, billing.PaymentStatus, billing.CreatedAt, billing.PaidAt));
        }
    }
}