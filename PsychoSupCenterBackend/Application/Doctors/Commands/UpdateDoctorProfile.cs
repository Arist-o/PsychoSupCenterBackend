using FluentValidation;
using MediatR;
using PsychoSupCenterBackend.Application.Common.Behaviors;
using PsychoSupCenterBackend.Application.Common.Interfaces;
using PsychoSupCenterBackend.Application.Common.Models;
using PsychoSupCenterBackend.Application.Doctors.DTOs;

namespace PsychoSupCenterBackend.Application.Doctors.Commands;

public static class UpdateDoctorProfile
{
    public sealed record Command(
        Guid DoctorProfileId,
        UpdateDoctorProfileDto Dto
    ) : ICommand<Result<DoctorProfileResponseDto>>;

    public sealed class Validator : AbstractValidator<Command>
    {
        public Validator()
        {
            RuleFor(x => x.DoctorProfileId)
                .NotEmpty();

            RuleFor(x => x.Dto.Bio)
                .MaximumLength(4000)
                .When(x => x.Dto.Bio is not null);

            RuleFor(x => x.Dto.CareerStartDate)
                .LessThan(DateTime.UtcNow)
                .WithMessage("Дата початку кар'єри не може бути в майбутньому.");
        }
    }

    public sealed class Handler(IUnitOfWork unitOfWork)
        : IRequestHandler<Command, Result<DoctorProfileResponseDto>>
    {
        public async Task<Result<DoctorProfileResponseDto>> Handle(
            Command request,
            CancellationToken cancellationToken)
        {
            var doctor = await unitOfWork.DoctorProfiles
                .GetByIdAsync(request.DoctorProfileId, cancellationToken);

            if (doctor is null)
                return Result<DoctorProfileResponseDto>.Failure(
                    $"Профіль лікаря з Id '{request.DoctorProfileId}' не знайдено.");

            doctor.Bio = request.Dto.Bio;
            doctor.CareerStartDate = request.Dto.CareerStartDate;
            doctor.UpdatedAt = DateTime.UtcNow;

            unitOfWork.DoctorProfiles.Update(doctor);

            var user = await unitOfWork.Users
                .GetByIdAsync(doctor.UserId, cancellationToken);

            return Result<DoctorProfileResponseDto>.Success(new DoctorProfileResponseDto(
                Id: doctor.Id,
                UserId: doctor.UserId,
                FirstName: user?.FirstName ?? string.Empty,
                LastName: user?.LastName ?? string.Empty,
                Email: user?.Email ?? string.Empty,
                PhotoUrl: user?.PhotoUrl,
                Bio: doctor.Bio,
                CareerStartDate: doctor.CareerStartDate,
                ExperienceYears: (int)((DateTime.UtcNow - doctor.CareerStartDate).TotalDays / 365.25),
                Status: doctor.Status,
                AverageRating: doctor.AverageRating,
                UpdatedAt: doctor.UpdatedAt
            ));
        }
    }
}