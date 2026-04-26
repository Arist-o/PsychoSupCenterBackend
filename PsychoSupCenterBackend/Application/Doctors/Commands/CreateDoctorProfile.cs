using FluentValidation;
using MediatR;
using PsychoSupCenterBackend.Application.Common.Behaviors;
using PsychoSupCenterBackend.Application.Common.Interfaces;
using PsychoSupCenterBackend.Application.Common.Models;
using PsychoSupCenterBackend.Application.Doctors.DTOs;
using PsychoSupCenterBackend.Domain.Entities;
using PsychoSupCenterBackend.Domain.Enums;

namespace PsychoSupCenterBackend.Application.Doctors.Commands;

public static class CreateDoctorProfile
{
    public sealed record Command(Guid UserId, CreateDoctorProfileDto Dto) : ICommand<Result<DoctorProfileResponseDto>>;

    public sealed class Validator : AbstractValidator<Command>
    {
        public Validator()
        {
            RuleFor(x => x.UserId).NotEmpty();
            RuleFor(x => x.Dto.Bio).MaximumLength(4000).When(x => x.Dto.Bio is not null);
            RuleFor(x => x.Dto.CareerStartDate).LessThanOrEqualTo(DateTime.UtcNow);
        }
    }

    public sealed class Handler(IUnitOfWork unitOfWork) : IRequestHandler<Command, Result<DoctorProfileResponseDto>>
    {
        public async Task<Result<DoctorProfileResponseDto>> Handle(Command request, CancellationToken cancellationToken)
        {
            var user = await unitOfWork.Users.GetByIdAsync(request.UserId, cancellationToken);
            if (user is null) return Result<DoctorProfileResponseDto>.Failure("Користувача не знайдено.");

            var exists = await unitOfWork.DoctorProfiles.AnyAsync(d => d.UserId == request.UserId, cancellationToken);
            if (exists) return Result<DoctorProfileResponseDto>.Failure("Профіль вже існує.");

            var profile = new DoctorProfile
            {
                Id = Guid.NewGuid(),
                UserId = request.UserId,
                Bio = request.Dto.Bio,
                CareerStartDate = request.Dto.CareerStartDate,
                Status = DoctorStatus.Active,
                AverageRating = 0.0,
                UpdatedAt = DateTime.UtcNow,
            };

            await unitOfWork.DoctorProfiles.AddAsync(profile, cancellationToken);

            return Result<DoctorProfileResponseDto>.Success(new DoctorProfileResponseDto(
                Id: profile.Id,
                UserId: profile.UserId,
                FirstName: user.FirstName,
                LastName: user.LastName,
                Email: user.Email,
                PhotoUrl: user.PhotoUrl,
                Bio: profile.Bio,
                CareerStartDate: profile.CareerStartDate,
                ExperienceYears: (int)((DateTime.UtcNow - profile.CareerStartDate).TotalDays / 365.25), 
                Status: profile.Status,
                AverageRating: profile.AverageRating,
                UpdatedAt: profile.UpdatedAt
            ));
        }
    }
}