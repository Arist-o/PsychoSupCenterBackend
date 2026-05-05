
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
            RuleFor(x => x.Dto.Name)
                .NotEmpty().WithMessage("Назва спеціалізації є обов'язковою.")
                .MaximumLength(200);

            RuleFor(x => x.Dto.Description)
                .MaximumLength(1000).WithMessage("Опис не може перевищувати 1000 символів.");
        }
    }

    public sealed class Handler(IUnitOfWork unitOfWork)
        : IRequestHandler<Command, Result<SpecializationResponseDto>>
    {
        public async Task<Result<SpecializationResponseDto>> Handle(
            Command request, CancellationToken cancellationToken)
        {
            var duplicate = await unitOfWork.DoctorSpecializations.AnyAsync(
                s => s.Name.ToLower() == request.Dto.Name.ToLower(),
                cancellationToken);

            if (duplicate)
                return Result<SpecializationResponseDto>.Failure(
                    $"Спеціалізація з назвою '{request.Dto.Name}' вже існує.");

            var spec = new DoctorSpecialization
            {
                Id = Guid.NewGuid(),
                Name = request.Dto.Name,
                Description = request.Dto.Description
            };

            await unitOfWork.DoctorSpecializations.AddAsync(spec, cancellationToken);

            return Result<SpecializationResponseDto>.Success(
                new SpecializationResponseDto(spec.Id, spec.Name, spec.Description));
        }
    }
}