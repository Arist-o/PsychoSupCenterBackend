using MediatR;
using Microsoft.EntityFrameworkCore;
using PsychoSupCenterBackend.Application.Billing.DTOs;
using PsychoSupCenterBackend.Application.Common.Behaviors;
using PsychoSupCenterBackend.Application.Common.Interfaces;
using PsychoSupCenterBackend.Application.Common.Models;

namespace PsychoSupCenterBackend.Application.Billing.Queries;

public static class GetAllBillings
{
    public sealed record Query : IQuery<Result<IReadOnlyList<BillingResponseDto>>>;

    public sealed class Handler(IUnitOfWork unitOfWork) : IRequestHandler<Query, Result<IReadOnlyList<BillingResponseDto>>>
    {
        public async Task<Result<IReadOnlyList<BillingResponseDto>>> Handle(Query request, CancellationToken cancellationToken)
        {
            var billings = await unitOfWork.Billings.Query()
                .Include(b => b.DoctorService)
                .Include(b => b.Appointment).ThenInclude(a => a.DoctorProfile).ThenInclude(d => d.User)
                .OrderByDescending(b => b.CreatedAt)
                .Select(b => new BillingResponseDto(
                    b.Id, 
                    b.DoctorServiceId, 
                    b.DoctorService.ServiceName,
                    b.Amount, 
                    b.PaymentStatus, 
                    b.CreatedAt, 
                    b.PaidAt,
                    b.Appointment != null ? $"{b.Appointment.DoctorProfile.User.FirstName} {b.Appointment.DoctorProfile.User.LastName}" : ""
                ))
                .ToListAsync(cancellationToken);

            return Result<IReadOnlyList<BillingResponseDto>>.Success(billings);
        }
    }
}