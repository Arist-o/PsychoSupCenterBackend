
using MediatR;
using Microsoft.EntityFrameworkCore;
using PsychoSupCenterBackend.Application.Common.Behaviors;
using PsychoSupCenterBackend.Application.Common.Interfaces;
using PsychoSupCenterBackend.Application.Common.Models;
using PsychoSupCenterBackend.Application.DoctorCertificates.DTOs;
using PsychoSupCenterBackend.Application.Doctors.DTOs;
using PsychoSupCenterBackend.Application.DoctorServices.DTOs;
using PsychoSupCenterBackend.Application.DoctorSpecializations.DTOs;

namespace PsychoSupCenterBackend.Application.Doctors.Queries;

public static class GetDoctorById
{
    public sealed record Query(Guid DoctorProfileId)
        : IQuery<Result<DoctorProfileResponseDto>>;

    public sealed class Handler(IUnitOfWork unitOfWork)
        : IRequestHandler<Query, Result<DoctorProfileResponseDto>>
    {
        public async Task<Result<DoctorProfileResponseDto>> Handle(
            Query request,
            CancellationToken cancellationToken)
        {
            var doctor = await unitOfWork.DoctorProfiles
                .Query()
                .Include(d => d.User)
                .Include(d => d.Specializations)
                .Include(d => d.Services)
                .Include(d => d.Certificates)
                .FirstOrDefaultAsync(
                    d => d.Id == request.DoctorProfileId,
                    cancellationToken);

            if (doctor is null)
                return Result<DoctorProfileResponseDto>.Failure(
                    $"Лікаря з Id '{request.DoctorProfileId}' не знайдено.");

            return Result<DoctorProfileResponseDto>.Success(new DoctorProfileResponseDto(
                Id: doctor.Id,
                UserId: doctor.UserId,
                FirstName: doctor.User.FirstName,
                LastName: doctor.User.LastName,
                Email: doctor.User.Email,
                Age: doctor.User.Age,
                PhotoUrl: doctor.User.PhotoUrl,
                Bio: doctor.Bio,
                CareerStartDate: doctor.CareerStartDate,
                ExperienceYears: doctor.ExperienceYears,
                Status: doctor.Status,
                AverageRating: doctor.AverageRating,
                UpdatedAt: doctor.UpdatedAt,
                Specializations: doctor.Specializations.Select(s => new DoctorSpecializationResponseDto(s.Id, s.Name, s.Description)).ToList(),
                Services: doctor.Services.Select(s => new DoctorServiceResponseDto(
                    s.Id, s.DoctorProfileId, s.ServiceName, s.Price, s.Description, s.DurationMinutes)).ToList(),
                Certificates: doctor.Certificates.Select(c => new DoctorCertificateResponseDto(
                    c.Id, c.DoctorProfileId, c.Name, c.IssuingOrganization, c.IssueDate, c.CertificateUrl, c.AddedAt)).ToList()
            ));
        }
    }
}