using FluentValidation;
using MediatR;
using PsychoSupCenterBackend.Application.Common.Behaviors;
using PsychoSupCenterBackend.Application.Common.Interfaces;
using PsychoSupCenterBackend.Application.Common.Models;
using PsychoSupCenterBackend.Application.Patients.DTOs;
using PsychoSupCenterBackend.Domain.Enums;

namespace PsychoSupCenterBackend.Application.Patients.Commands;

public static class UpdatePatientProfile
{
    public sealed record Command(
        Guid PatientProfileId,
        UpdatePatientProfileDto Dto
    ) : ICommand<Result<PatientProfileResponseDto>>;

    public sealed class Validator : AbstractValidator<Command>
    {
        public Validator()
        {
            RuleFor(x => x.PatientProfileId).NotEmpty();
            RuleFor(x => x.Dto.Type).IsInEnum();
            RuleFor(x => x.Dto.MilitaryId)
                .NotEmpty().When(x => x.Dto.Type == PatientType.Military);
            RuleFor(x => x.Dto.DateOfBirth).LessThan(DateTime.UtcNow);
        }
    }

    public sealed class Handler(IUnitOfWork unitOfWork)
        : IRequestHandler<Command, Result<PatientProfileResponseDto>>
    {
        public async Task<Result<PatientProfileResponseDto>> Handle(
            Command request, CancellationToken cancellationToken)
        {
            var patient = await unitOfWork.PatientProfiles.GetByIdAsync(request.PatientProfileId, cancellationToken);

            if (patient is null)
                return Result<PatientProfileResponseDto>.Failure($"Профіль пацієнта з Id '{request.PatientProfileId}' не знайдено.");

            patient.Type = request.Dto.Type;
            patient.MilitaryId = request.Dto.MilitaryId;
            patient.EmergencyContact = request.Dto.EmergencyContact;
            patient.DateOfBirth = request.Dto.DateOfBirth;

            unitOfWork.PatientProfiles.Update(patient);

            var user = await unitOfWork.Users.GetByIdAsync(patient.UserId, cancellationToken);

            return Result<PatientProfileResponseDto>.Success(new PatientProfileResponseDto(
                patient.Id, patient.UserId, user?.FirstName ?? "", user?.LastName ?? "", user?.Email ?? "", user?.PhotoUrl,
                patient.Type, patient.MilitaryId, patient.EmergencyContact, patient.DateOfBirth,
                (int)((DateTime.UtcNow - patient.DateOfBirth).TotalDays / 365.25)));
        }
    }
}