
using FluentValidation;
using MediatR;
using Microsoft.EntityFrameworkCore;
using PsychoSupCenterBackend.Application.Common.Behaviors;
using PsychoSupCenterBackend.Application.Common.Interfaces;
using PsychoSupCenterBackend.Application.Common.Models;
using PsychoSupCenterBackend.Application.DoctorSpecializations.DTOs;
using PsychoSupCenterBackend.Domain.Entities;

namespace PsychoSupCenterBackend.Application.DoctorSpecializations.Commands;

public static class AssignSpecializationToDoctor
{
    public sealed record Command(AssignSpecializationDto Dto) : ICommand<Result<SpecializationResponseDto>>;

    public sealed class Validator : AbstractValidator<Command>
    {
        public Validator()
        {
            RuleFor(x => x.Dto.DoctorProfileId).NotEmpty();
            RuleFor(x => x.Dto.Name)
                .NotEmpty().WithMessage("Назва спеціалізації є обов'язковою.")
                .MaximumLength(200);
        }
    }

    public sealed class Handler(IUnitOfWork unitOfWork)
        : IRequestHandler<Command, Result<SpecializationResponseDto>>
    {
        public async Task<Result<SpecializationResponseDto>> Handle(
            Command request,
            CancellationToken cancellationToken)
        {
            var doctor = await unitOfWork.DoctorProfiles.Query()
                .Include(d => d.Specializations)
                .FirstOrDefaultAsync(d => d.Id == request.Dto.DoctorProfileId, cancellationToken);

            if (doctor is null)
                return Result<SpecializationResponseDto>.Failure(
                    $"Лікаря з Id '{request.Dto.DoctorProfileId}' не знайдено.");

            var specialization = await unitOfWork.DoctorSpecializations.FirstOrDefaultAsync(
                s => s.Name.ToLower() == request.Dto.Name.ToLower(),
                cancellationToken);

            if (specialization is null)
                return Result<SpecializationResponseDto>.Failure(
                    $"Спеціалізацію '{request.Dto.Name}' не знайдено в довіднику.");

            if (doctor.Specializations.Any(s => s.Id == specialization.Id))
                return Result<SpecializationResponseDto>.Failure(
                    $"Спеціалізація '{request.Dto.Name}' вже призначена цьому лікарю.");

            doctor.Specializations.Add(specialization);
            
            await unitOfWork.SaveChangesAsync(cancellationToken);

            return Result<SpecializationResponseDto>.Success(
                new SpecializationResponseDto(
                    specialization.Id,
                    specialization.Name,
                    specialization.Description));
        }
    }
}