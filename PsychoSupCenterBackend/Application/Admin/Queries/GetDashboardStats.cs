using MediatR;
using Microsoft.EntityFrameworkCore;
using PsychoSupCenterBackend.Application.Admin.DTOs;
using PsychoSupCenterBackend.Application.Common.Interfaces;
using PsychoSupCenterBackend.Application.Common.Models;
using PsychoSupCenterBackend.Domain.Enums;
using System.Globalization;

namespace PsychoSupCenterBackend.Application.Admin.Queries;

public class GetDashboardStats
{
    public sealed record Query : IRequest<Result<DashboardStatsDto>>;

    public sealed class Handler : IRequestHandler<Query, Result<DashboardStatsDto>>
    {
        private readonly IUnitOfWork _unitOfWork;

        public Handler(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task<Result<DashboardStatsDto>> Handle(Query request, CancellationToken cancellationToken)
        {
            var patients = await _unitOfWork.PatientProfiles.GetAllAsync(cancellationToken);
            var doctors = await _unitOfWork.DoctorProfiles.GetAllAsync(cancellationToken);
            var appointments = await _unitOfWork.Appointments.GetAllAsync(cancellationToken);
            var billings = await _unitOfWork.Billings.GetAllAsync(cancellationToken);

            var totalRevenue = billings.Where(b => b.PaymentStatus == PaymentStatus.Paid).Sum(b => b.Amount);

            var currentYear = DateTime.UtcNow.Year;
            
            var monthlyStats = appointments
                .Where(a => a.ScheduledAt.Year == currentYear)
                .GroupBy(a => a.ScheduledAt.Month)
                .Select(g => new MonthlyStatDto(
                    CultureInfo.CurrentCulture.DateTimeFormat.GetAbbreviatedMonthName(g.Key),
                    g.Count()
                ))
                .ToList();

            var allMonths = Enumerable.Range(1, 12).Select(m => CultureInfo.CurrentCulture.DateTimeFormat.GetAbbreviatedMonthName(m)).ToList();
            var completeMonthlyStats = allMonths.Select(month => 
            {
                var stat = monthlyStats.FirstOrDefault(m => m.Month == month);
                return stat ?? new MonthlyStatDto(month, 0);
            }).ToList();

            var result = new DashboardStatsDto(
                patients.Count,
                doctors.Count,
                appointments.Count,
                totalRevenue,
                completeMonthlyStats
            );

            return Result<DashboardStatsDto>.Success(result);
        }
    }
}
