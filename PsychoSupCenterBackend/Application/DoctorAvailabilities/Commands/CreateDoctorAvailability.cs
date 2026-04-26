
using FluentValidation;
using MediatR;
using PsychoSupCenterBackend.Application.Common.Behaviors;
using PsychoSupCenterBackend.Application.Common.Interfaces;
using PsychoSupCenterBackend.Application.Common.Models;
using PsychoSupCenterBackend.Application.DoctorAvailabilities.DTOs;
using PsychoSupCenterBackend.Domain.Entities;

namespace PsychoSupCenterBackend.Application.DoctorAvailabilities.Commands;

public static class CreateDoctorAvailability
{
    public sealed record Command(CreateDoctorAvailabilityDto Dto)
        : ICommand<Result<DoctorAvailabilityResponseDto>>;

    public sealed class Validator : AbstractValidator<Command>
    {
        public Validator()
        {
            RuleFor(x => x.Dto.DoctorProfileId).NotEmpty();
            RuleFor(x => x.Dto.StartTime)
                .LessThan(x => x.Dto.EndTime)
                .WithMessage("Час початку має бути раніше часу кінця.");
        }
    }

    public sealed class Handler(IUnitOfWork unitOfWork)
        : IRequestHandler<Command, Result<DoctorAvailabilityResponseDto>>
    {
        public async Task<Result<DoctorAvailabilityResponseDto>> Handle(
            Command request, CancellationToken cancellationToken)
        {
            var doctorExists = await unitOfWork.DoctorProfiles
                .AnyAsync(d => d.Id == request.Dto.DoctorProfileId, cancellationToken);

            if (!doctorExists)
                return Result<DoctorAvailabilityResponseDto>.Failure("Лікаря не знайдено.");

            var overlap = await unitOfWork.DoctorAvailabilities.AnyAsync(
                a => a.DoctorProfileId == request.Dto.DoctorProfileId
                  && a.Day == request.Dto.Day
                  && a.StartTime < request.Dto.EndTime
                  && a.EndTime > request.Dto.StartTime,
                cancellationToken);

            if (overlap)
                return Result<DoctorAvailabilityResponseDto>.Failure(
                    "Цей часовий слот перетинається з існуючим розкладом.");

            var availability = new DoctorAvailability
            {
                Id = Guid.NewGuid(),
                DoctorProfileId = request.Dto.DoctorProfileId,
                Day = request.Dto.Day,
                StartTime = request.Dto.StartTime,
                EndTime = request.Dto.EndTime,
            };

            await unitOfWork.DoctorAvailabilities.AddAsync(availability, cancellationToken);

            return Result<DoctorAvailabilityResponseDto>.Success(
                new DoctorAvailabilityResponseDto(
                    availability.Id, availability.DoctorProfileId,
                    availability.Day, availability.StartTime, availability.EndTime));
        }
    }
}