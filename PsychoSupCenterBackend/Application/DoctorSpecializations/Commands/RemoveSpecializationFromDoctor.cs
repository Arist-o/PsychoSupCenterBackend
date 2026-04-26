using FluentValidation;
using MediatR;
using PsychoSupCenterBackend.Application.Common.Behaviors;
using PsychoSupCenterBackend.Application.Common.Interfaces;
using PsychoSupCenterBackend.Application.Common.Models;

namespace PsychoSupCenterBackend.Application.DoctorSpecializations.Commands;

public static class RemoveSpecializationFromDoctor
{
    public sealed record Command(
        Guid DoctorProfileId,
        string Name
    ) : ICommand<Result<bool>>;

    public sealed class Validator : AbstractValidator<Command>
    {
        public Validator()
        {
            RuleFor(x => x.DoctorProfileId).NotEmpty();
            RuleFor(x => x.Name).NotEmpty();
        }
    }

    public sealed class Handler(IUnitOfWork unitOfWork)
        : IRequestHandler<Command, Result<bool>>
    {
        public async Task<Result<bool>> Handle(
            Command request, CancellationToken cancellationToken)
        {
            var spec = await unitOfWork.DoctorSpecializations.FirstOrDefaultAsync(
                s => s.DoctorProfileId == request.DoctorProfileId
                  && s.Name.ToLower() == request.Name.ToLower(),
                cancellationToken);

            if (spec is null)
                return Result<bool>.Failure(
                    $"Спеціалізацію '{request.Name}' не знайдено у цього лікаря.");

            unitOfWork.DoctorSpecializations.Remove(spec);
            return Result<bool>.Success(true);
        }
    }
}