
using FluentValidation;
using MediatR;
using PsychoSupCenterBackend.Application.Common.Behaviors;
using PsychoSupCenterBackend.Application.Common.Interfaces;
using PsychoSupCenterBackend.Application.Common.Models;

namespace PsychoSupCenterBackend.Application.DoctorSpecializations.Commands;

public static class DeleteSpecialization
{
    public sealed record Command(Guid SpecializationId) : ICommand<Result<bool>>;

    public sealed class Validator : AbstractValidator<Command>
    {
        public Validator() => RuleFor(x => x.SpecializationId).NotEmpty();
    }

    public sealed class Handler(IUnitOfWork unitOfWork)
        : IRequestHandler<Command, Result<bool>>
    {
        public async Task<Result<bool>> Handle(
            Command request, CancellationToken cancellationToken)
        {
            var spec = await unitOfWork.DoctorSpecializations
                .GetByIdAsync(request.SpecializationId, cancellationToken);

            if (spec is null)
                return Result<bool>.Failure("Спеціалізацію не знайдено.");

            unitOfWork.DoctorSpecializations.Remove(spec);
            return Result<bool>.Success(true);
        }
    }
}