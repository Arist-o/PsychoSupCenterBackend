using FluentValidation;
using MediatR;
using PsychoSupCenterBackend.Application.Common.Behaviors;
using PsychoSupCenterBackend.Application.Common.Interfaces;
using PsychoSupCenterBackend.Application.Common.Models;
using PsychoSupCenterBackend.Application.DoctorServices.DTOs;

namespace PsychoSupCenterBackend.Application.DoctorServices.Queries;

public static class GetDoctorServiceById
{
    public sealed record Query(Guid ServiceId) : IQuery<Result<DoctorServiceResponseDto>>;

    public sealed class Validator : AbstractValidator<Query>
    {
        public Validator() => RuleFor(x => x.ServiceId).NotEmpty();
    }

    public sealed class Handler(IUnitOfWork unitOfWork)
        : IRequestHandler<Query, Result<DoctorServiceResponseDto>>
    {
        public async Task<Result<DoctorServiceResponseDto>> Handle(
            Query request, CancellationToken cancellationToken)
        {
            var service = await unitOfWork.DoctorServices.GetByIdAsync(request.ServiceId, cancellationToken);

            if (service is null)
                return Result<DoctorServiceResponseDto>.Failure("Послугу не знайдено.");

            return Result<DoctorServiceResponseDto>.Success(new DoctorServiceResponseDto(
                service.Id, service.DoctorProfileId, service.ServiceName, service.Price));
        }
    }
}