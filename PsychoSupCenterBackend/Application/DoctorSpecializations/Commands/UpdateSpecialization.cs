
using Application.DoctorSpecializations.DTOs;
using FluentValidation;
using MediatR;
using PsychoSupCenterBackend.Application.Common.Behaviors;
using PsychoSupCenterBackend.Application.Common.Interfaces;
using PsychoSupCenterBackend.Application.Common.Models;
using PsychoSupCenterBackend.Application.DoctorSpecializations.DTOs;

namespace PsychoSupCenterBackend.Application.DoctorSpecializations.Commands;

public static class UpdateSpecialization
{
    public sealed record Command(Guid SpecializationId, UpdateSpecializationDto Dto) : ICommand<Result<SpecializationResponseDto>>;

    public sealed class Validator : AbstractValidator<Command>
    {
        public Validator()
        {
            RuleFor(x => x.SpecializationId).NotEmpty();
            RuleFor(x => x.Dto.NewName).NotEmpty().MaximumLength(200);
        }
    }

    public sealed class Handler(IUnitOfWork unitOfWork)
        : IRequestHandler<Command, Result<SpecializationResponseDto>>
    {
        public async Task<Result<SpecializationResponseDto>> Handle(
            Command request, CancellationToken cancellationToken)
        {
            var spec = await unitOfWork.DoctorSpecializations
                .GetByIdAsync(request.SpecializationId, cancellationToken);

            if (spec is null)
                return Result<SpecializationResponseDto>.Failure("Спеціалізацію не знайдено.");

            spec.Name = request.Dto.NewName;
            unitOfWork.DoctorSpecializations.Update(spec);

            return Result<SpecializationResponseDto>.Success(
                new SpecializationResponseDto(spec.Id, spec.DoctorProfileId, spec.Name));
        }
    }
}