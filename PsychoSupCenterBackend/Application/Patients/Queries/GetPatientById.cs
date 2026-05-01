using MediatR;
using PsychoSupCenterBackend.Application.Common.Interfaces;
using PsychoSupCenterBackend.Application.Common.Models;
using PsychoSupCenterBackend.Application.Patients.DTOs;
using System;
using System.Collections.Generic;
using System.Text;

namespace Application.Patients.Queries
{
    public class GetPatientById
    {
        public sealed record Query(Guid Id) : IRequest<Result<PatientProfileResponseDto>>;
       
        public sealed class Handler(IUnitOfWork _unitOfWork) : IRequestHandler<Query, Result<PatientProfileResponseDto>>
        {
            public async Task<Result<PatientProfileResponseDto>> Handle(Query request, CancellationToken cancellationToken)
            {
                var patient = await _unitOfWork.PatientProfiles.GetByIdAsync(request.Id);
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
                Age: (int)((DateTime.UtcNow - patient.DateOfBirth).TotalDays / 365.25)
            ));
            }
        }
    }
}