using MediatR;
using Microsoft.EntityFrameworkCore;
using PsychoSupCenterBackend.Application.Common.Interfaces;
using PsychoSupCenterBackend.Application.Common.Models;
using PsychoSupCenterBackend.Application.Patients.DTOs;

namespace Application.Patients.Queries
{
    public class GetPatientById
    {
        public sealed record Query(Guid Id) : IRequest<Result<PatientProfileResponseDto>>;
       
        public sealed class Handler(IUnitOfWork _unitOfWork) : IRequestHandler<Query, Result<PatientProfileResponseDto>>
        {
            public async Task<Result<PatientProfileResponseDto>> Handle(Query request, CancellationToken cancellationToken)
            {
                var patient = await _unitOfWork.PatientProfiles.Query()
                    .Include(p => p.User)
                    .FirstOrDefaultAsync(p => p.Id == request.Id, cancellationToken);
                    
                if (patient == null)
                {
                    return Result<PatientProfileResponseDto>.Failure($"Пацієнта з Id '{request.Id}' не знайдено.");
                }
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
                    Age: patient.User.Age
                ));
            }
        }
    }
}