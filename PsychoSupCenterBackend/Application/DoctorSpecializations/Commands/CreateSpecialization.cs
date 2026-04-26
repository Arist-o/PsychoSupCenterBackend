
using Application.DoctorSpecializations.DTOs;
using FluentValidation;
using MediatR;
using PsychoSupCenterBackend.Application.Common.Behaviors;
using PsychoSupCenterBackend.Application.Common.Interfaces;
using PsychoSupCenterBackend.Application.Common.Models;
using PsychoSupCenterBackend.Application.DoctorSpecializations.DTOs;
using PsychoSupCenterBackend.Domain.Entities;

namespace PsychoSupCenterBackend.Application.DoctorSpecializations.Commands;

public static class CreateSpecialization
{
    public sealed record Command(CreateSpecializationDto Dto) : ICommand<Result<SpecializationResponseDto>>;

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
            Command request, CancellationToken cancellationToken)
        {
            var doctorExists = await unitOfWork.DoctorProfiles
                .AnyAsync(d => d.Id == request.Dto.DoctorProfileId, cancellationToken);

            if (!doctorExists)
                return Result<SpecializationResponseDto>.Failure("Лікаря не знайдено.");

            var duplicate = await unitOfWork.DoctorSpecializations.AnyAsync(
                s => s.DoctorProfileId == request.Dto.DoctorProfileId
                  && s.Name.ToLower() == request.Dto.Name.ToLower(),
                cancellationToken);

            if (duplicate)
                return Result<SpecializationResponseDto>.Failure(
                    $"Спеціалізація '{request.Dto.Name}' вже існує у цього лікаря.");

            var spec = new DoctorSpecialization
            {
                Id = Guid.NewGuid(),
                DoctorProfileId = request.Dto.DoctorProfileId,
                Name = request.Dto.Name,
            };

            await unitOfWork.DoctorSpecializations.AddAsync(spec, cancellationToken);

            return Result<SpecializationResponseDto>.Success(
                new SpecializationResponseDto(spec.Id, spec.DoctorProfileId, spec.Name));
        }
    }
}