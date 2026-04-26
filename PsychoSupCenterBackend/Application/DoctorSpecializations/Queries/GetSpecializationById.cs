
using FluentValidation;
using MediatR;
using PsychoSupCenterBackend.Application.Common.Behaviors;
using PsychoSupCenterBackend.Application.Common.Interfaces;
using PsychoSupCenterBackend.Application.Common.Models;
using PsychoSupCenterBackend.Application.DoctorSpecializations.DTOs;

namespace PsychoSupCenterBackend.Application.DoctorSpecializations.Queries;

public static class GetSpecializationById
{
    public sealed record Query(Guid SpecializationId)
        : IQuery<Result<SpecializationResponseDto>>;

    public sealed class Validator : AbstractValidator<Query>
    {
        public Validator() => RuleFor(x => x.SpecializationId).NotEmpty();
    }

    public sealed class Handler(IUnitOfWork unitOfWork)
        : IRequestHandler<Query, Result<SpecializationResponseDto>>
    {
        public async Task<Result<SpecializationResponseDto>> Handle(
            Query request, CancellationToken cancellationToken)
        {
            var spec = await unitOfWork.DoctorSpecializations
                .GetByIdAsync(request.SpecializationId, cancellationToken);

            if (spec is null)
                return Result<SpecializationResponseDto>.Failure("Спеціалізацію не знайдено.");

            return Result<SpecializationResponseDto>.Success(
                new SpecializationResponseDto(spec.Id, spec.DoctorProfileId, spec.Name));
        }
    }
}