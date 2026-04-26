using FluentValidation;
using MediatR;
using PsychoSupCenterBackend.Application.Common.Behaviors;
using PsychoSupCenterBackend.Application.Common.Interfaces;
using PsychoSupCenterBackend.Application.Common.Models;
using PsychoSupCenterBackend.Application.Patients.DTOs;
using PsychoSupCenterBackend.Domain.Entities;
using PsychoSupCenterBackend.Domain.Enums;

namespace PsychoSupCenterBackend.Application.Patients.Commands;

public static class CreatePatientProfile
{
    public sealed record Command(
        Guid UserId,
        CreatePatientProfileDto Dto
    ) : ICommand<Result<PatientProfileResponseDto>>;

    public sealed class Validator : AbstractValidator<Command>
    {
        public Validator()
        {
            RuleFor(x => x.UserId).NotEmpty();
            RuleFor(x => x.Dto.Type).IsInEnum();
            RuleFor(x => x.Dto.MilitaryId)
                .NotEmpty().When(x => x.Dto.Type == PatientType.Military)
                .WithMessage("MilitaryId є обов'язковим для військових.");
            RuleFor(x => x.Dto.DateOfBirth).LessThan(DateTime.UtcNow);
        }
    }

    public sealed class Handler(IUnitOfWork unitOfWork)
        : IRequestHandler<Command, Result<PatientProfileResponseDto>>
    {
        public async Task<Result<PatientProfileResponseDto>> Handle(
            Command request, CancellationToken cancellationToken)
        {
            var user = await unitOfWork.Users.GetByIdAsync(request.UserId, cancellationToken);
            if (user is null)
                return Result<PatientProfileResponseDto>.Failure("Користувача не знайдено.");

            var exists = await unitOfWork.PatientProfiles.AnyAsync(p => p.UserId == request.UserId, cancellationToken);
            if (exists)
                return Result<PatientProfileResponseDto>.Failure("Профіль пацієнта вже існує для цього користувача.");

            var patient = new PatientProfile
            {
                Id = Guid.NewGuid(),
                UserId = request.UserId,
                Type = request.Dto.Type,
                MilitaryId = request.Dto.MilitaryId,
                EmergencyContact = request.Dto.EmergencyContact,
                DateOfBirth = request.Dto.DateOfBirth
            };

            await unitOfWork.PatientProfiles.AddAsync(patient, cancellationToken);

            return Result<PatientProfileResponseDto>.Success(new PatientProfileResponseDto(
                patient.Id, patient.UserId, user.FirstName, user.LastName, user.Email, user.PhotoUrl,
                patient.Type, patient.MilitaryId, patient.EmergencyContact, patient.DateOfBirth,
                (int)((DateTime.UtcNow - patient.DateOfBirth).TotalDays / 365.25)));
        }
    }
}