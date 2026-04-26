
using MediatR;
using Microsoft.EntityFrameworkCore;
using PsychoSupCenterBackend.Application.Common.Behaviors;
using PsychoSupCenterBackend.Application.Common.Interfaces;
using PsychoSupCenterBackend.Application.Common.Models;
using PsychoSupCenterBackend.Application.Doctors.DTOs;

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
                PhotoUrl: doctor.User.PhotoUrl,
                Bio: doctor.Bio,
                CareerStartDate: doctor.CareerStartDate,
                ExperienceYears: doctor.ExperienceYears,
                Status: doctor.Status,
                AverageRating: doctor.AverageRating,
                UpdatedAt: doctor.UpdatedAt
            ));
        }
    }
}