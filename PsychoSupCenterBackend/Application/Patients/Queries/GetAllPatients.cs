// Application/Patients/Queries/GetAllPatients.cs
using MediatR;
using Microsoft.EntityFrameworkCore;
using PsychoSupCenterBackend.Application.Common.Behaviors;
using PsychoSupCenterBackend.Application.Common.Interfaces;
using PsychoSupCenterBackend.Application.Common.Models;
using PsychoSupCenterBackend.Application.Patients.DTOs;

namespace PsychoSupCenterBackend.Application.Patients.Queries;

public static class GetAllPatients
{
    public sealed record Query(int Page = 1, int PageSize = 20)
        : IQuery<Result<IReadOnlyList<PatientProfileResponseDto>>>;

    public sealed class Handler(IUnitOfWork unitOfWork)
        : IRequestHandler<Query, Result<IReadOnlyList<PatientProfileResponseDto>>>
    {
        public async Task<Result<IReadOnlyList<PatientProfileResponseDto>>> Handle(
            Query request, CancellationToken cancellationToken)
        {
            var patients = await unitOfWork.PatientProfiles
                .Query()
                .Include(p => p.User)
                .Where(p => p.User.IsActive)
                .OrderBy(p => p.User.LastName)
                .Skip((request.Page - 1) * request.PageSize)
                .Take(request.PageSize)
                .Select(p => new PatientProfileResponseDto(
                    p.Id, p.UserId,
                    p.User.FirstName, p.User.LastName,
                    p.User.Email, p.User.PhotoUrl,
                    p.Type, p.MilitaryId, p.EmergencyContact,
                    p.DateOfBirth,
                    (int)((DateTime.UtcNow - p.DateOfBirth).TotalDays / 365.25)))
                .ToListAsync(cancellationToken);

            return Result<IReadOnlyList<PatientProfileResponseDto>>.Success(patients);
        }
    }
}