using FluentValidation;
using MediatR;
using PsychoSupCenterBackend.Application.Common.Behaviors;
using PsychoSupCenterBackend.Application.Common.Interfaces;
using PsychoSupCenterBackend.Application.Common.Models;
using PsychoSupCenterBackend.Application.DoctorServices.DTOs;
using PsychoSupCenterBackend.Domain.Entities;

namespace PsychoSupCenterBackend.Application.DoctorServices.Commands;

public static class CreateDoctorService
{
    public sealed record Command(
        Guid DoctorProfileId,
        CreateDoctorServiceDto Dto
    ) : ICommand<Result<DoctorServiceResponseDto>>;

    public sealed class Validator : AbstractValidator<Command>
    {
        public Validator()
        {
            RuleFor(x => x.DoctorProfileId).NotEmpty();

            RuleFor(x => x.Dto.ServiceName)
                .NotEmpty().WithMessage("Назва послуги є обов'язковою.")
                .MaximumLength(200);

            RuleFor(x => x.Dto.Price)
                .GreaterThan(0).WithMessage("Ціна має бути більше нуля.");
        }
    }

    public sealed class Handler(IUnitOfWork unitOfWork)
        : IRequestHandler<Command, Result<DoctorServiceResponseDto>>
    {
        public async Task<Result<DoctorServiceResponseDto>> Handle(
            Command request, CancellationToken cancellationToken)
        {
            var doctorExists = await unitOfWork.DoctorProfiles
                .AnyAsync(d => d.Id == request.DoctorProfileId, cancellationToken);

            if (!doctorExists)
                return Result<DoctorServiceResponseDto>.Failure(
                    $"Лікаря з Id '{request.DoctorProfileId}' не знайдено.");

            var service = new DoctorService
            {
                Id = Guid.NewGuid(),
                DoctorProfileId = request.DoctorProfileId,
                ServiceName = request.Dto.ServiceName, 
                Price = request.Dto.Price,             
            };

            await unitOfWork.DoctorServices.AddAsync(service, cancellationToken);

            return Result<DoctorServiceResponseDto>.Success(new DoctorServiceResponseDto(
                service.Id, service.DoctorProfileId, service.ServiceName, service.Price));
        }
    }
}