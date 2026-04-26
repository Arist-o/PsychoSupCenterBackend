
using FluentValidation;
using MediatR;
using Microsoft.EntityFrameworkCore;
using PsychoSupCenterBackend.Application.Billing.DTOs;
using PsychoSupCenterBackend.Application.Common.Behaviors;
using PsychoSupCenterBackend.Application.Common.Interfaces;
using PsychoSupCenterBackend.Application.Common.Models;

namespace PsychoSupCenterBackend.Application.Billing.Queries;

public static class GetBillingsByPatientId
{
    public sealed record Query(Guid PatientProfileId) : IQuery<Result<IReadOnlyList<BillingResponseDto>>>;

    public sealed class Validator : AbstractValidator<Query>
    {
        public Validator() => RuleFor(x => x.PatientProfileId).NotEmpty();
    }

    public sealed class Handler(IUnitOfWork unitOfWork)
        : IRequestHandler<Query, Result<IReadOnlyList<BillingResponseDto>>>
    {
        public async Task<Result<IReadOnlyList<BillingResponseDto>>> Handle(
            Query request, CancellationToken cancellationToken)
        {
            var billings = await unitOfWork.Billings
                .Query()
                .Include(b => b.DoctorService)
                .Include(b => b.Appointment)
                .Where(b => b.Appointment != null
                         && b.Appointment.PatientProfileId == request.PatientProfileId)
                .OrderByDescending(b => b.CreatedAt)
                .Select(b => new BillingResponseDto(
                    b.Id, b.DoctorServiceId, b.DoctorService.ServiceName,
                    b.Amount, b.PaymentStatus, b.CreatedAt, b.PaidAt))
                .ToListAsync(cancellationToken);

            return Result<IReadOnlyList<BillingResponseDto>>.Success(billings);
        }
    }
}