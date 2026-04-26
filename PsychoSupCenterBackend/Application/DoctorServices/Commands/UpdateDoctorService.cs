using FluentValidation;
using MediatR;
using PsychoSupCenterBackend.Application.Common.Behaviors;
using PsychoSupCenterBackend.Application.Common.Interfaces;
using PsychoSupCenterBackend.Application.Common.Models;
using PsychoSupCenterBackend.Application.DoctorServices.DTOs;

namespace PsychoSupCenterBackend.Application.DoctorServices.Commands;

public static class UpdateDoctorService
{
    public sealed record Command(
        Guid ServiceId,
        UpdateDoctorServiceDto Dto
    ) : ICommand<Result<DoctorServiceResponseDto>>;

    public sealed class Validator : AbstractValidator<Command>
    {
        public Validator()
        {
            RuleFor(x => x.ServiceId).NotEmpty();
            RuleFor(x => x.Dto.ServiceName).NotEmpty().MaximumLength(200);
            RuleFor(x => x.Dto.Price).GreaterThan(0);
        }
    }

    public sealed class Handler(IUnitOfWork unitOfWork)
        : IRequestHandler<Command, Result<DoctorServiceResponseDto>>
    {
        public async Task<Result<DoctorServiceResponseDto>> Handle(
            Command request, CancellationToken cancellationToken)
        {
            var service = await unitOfWork.DoctorServices
                .GetByIdAsync(request.ServiceId, cancellationToken);

            if (service is null)
                return Result<DoctorServiceResponseDto>.Failure($"Послугу з Id '{request.ServiceId}' не знайдено.");

            service.ServiceName = request.Dto.ServiceName;
            service.Price = request.Dto.Price;

            unitOfWork.DoctorServices.Update(service);

            return Result<DoctorServiceResponseDto>.Success(new DoctorServiceResponseDto(
                service.Id, service.DoctorProfileId, service.ServiceName, service.Price));
        }
    }
}