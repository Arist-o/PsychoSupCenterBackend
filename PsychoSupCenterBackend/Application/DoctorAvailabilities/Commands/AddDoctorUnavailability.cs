
using Application.DoctorAvailabilities.DTOs;
using FluentValidation;
using MediatR;
using PsychoSupCenterBackend.Application.Common.Behaviors;
using PsychoSupCenterBackend.Application.Common.Interfaces;
using PsychoSupCenterBackend.Application.Common.Models;
using PsychoSupCenterBackend.Application.DoctorAvailabilities.DTOs;
using PsychoSupCenterBackend.Domain.Entities;

namespace PsychoSupCenterBackend.Application.DoctorAvailabilities.Commands;

public static class AddDoctorUnavailability
{
    public sealed record Command(AddDoctorUnavailabilityDto Dto) : ICommand<Result<DoctorUnavailabilityResponseDto>>;

    public sealed class Validator : AbstractValidator<Command>
    {
        public Validator()
        {
            RuleFor(x => x.Dto.DoctorProfileId).NotEmpty();
            RuleFor(x => x.Dto.StartDate).LessThan(x => x.Dto.EndDate)
                .WithMessage("Дата початку має бути раніше дати кінця.");
            RuleFor(x => x.Dto.Reason).MaximumLength(500).When(x => x.Dto.Reason is not null);
        }
    }

    public sealed class Handler(IUnitOfWork unitOfWork)
        : IRequestHandler<Command, Result<DoctorUnavailabilityResponseDto>>
    {
        public async Task<Result<DoctorUnavailabilityResponseDto>> Handle(
            Command request, CancellationToken cancellationToken)
        {
            var doctorExists = await unitOfWork.DoctorProfiles
                .AnyAsync(d => d.Id == request.Dto.DoctorProfileId, cancellationToken);

            if (!doctorExists)
                return Result<DoctorUnavailabilityResponseDto>.Failure("Лікаря не знайдено.");

            var unavailability = new DoctorUnavailability
            {
                Id = Guid.NewGuid(),
                DoctorProfileId = request.Dto.DoctorProfileId,
                StartDate = request.Dto.StartDate,
                EndDate = request.Dto.EndDate,
                Reason = request.Dto.Reason,
            };

            await unitOfWork.DoctorUnavailabilities.AddAsync(unavailability, cancellationToken);

            return Result<DoctorUnavailabilityResponseDto>.Success(
                new DoctorUnavailabilityResponseDto(
                    unavailability.Id, unavailability.DoctorProfileId,
                    unavailability.StartDate, unavailability.EndDate, unavailability.Reason));
        }
    }
}