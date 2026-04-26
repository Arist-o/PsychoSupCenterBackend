
using MediatR;
using Microsoft.EntityFrameworkCore;
using PsychoSupCenterBackend.Application.Common.Behaviors;
using PsychoSupCenterBackend.Application.Common.Interfaces;
using PsychoSupCenterBackend.Application.Common.Models;
using PsychoSupCenterBackend.Application.Patients.DTOs;

namespace PsychoSupCenterBackend.Application.Patients.Queries;

public static class GetPatientProfileById
{
    public sealed record Query(Guid PatientProfileId)
        : IQuery<Result<PatientProfileResponseDto>>;

    public sealed class Handler(IUnitOfWork unitOfWork)
        : IRequestHandler<Query, Result<PatientProfileResponseDto>>
    {
        public async Task<Result<PatientProfileResponseDto>> Handle(
            Query request,
            CancellationToken cancellationToken)
        {
            var patient = await unitOfWork.PatientProfiles
                .Query()
                .Include(p => p.User)
                .FirstOrDefaultAsync(
                    p => p.Id == request.PatientProfileId,
                    cancellationToken);

            if (patient is null)
                return Result<PatientProfileResponseDto>.Failure(
                    $"Пацієнта з Id '{request.PatientProfileId}' не знайдено.");

            return Result<PatientProfileResponseDto>.Success(new PatientProfileResponseDto(
                Id: patient.Id,
                UserId: patient.UserId,
                FirstName: patient.User.FirstName,
                LastName: patient.User.LastName,
                Email: patient.User.Email,
                PhotoUrl: patient.User.PhotoUrl,
                Type: patient.Type,
                MilitaryId: patient.MilitaryId,
                EmergencyContact: patient.EmergencyContact,
                DateOfBirth: patient.DateOfBirth,
                Age: (int)((DateTime.UtcNow - patient.DateOfBirth).TotalDays / 365.25)
            ));
        }
    }
}