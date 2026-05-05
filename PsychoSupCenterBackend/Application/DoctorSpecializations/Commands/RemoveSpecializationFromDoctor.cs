using FluentValidation;
using MediatR;
using Microsoft.EntityFrameworkCore;
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
            var doctor = await unitOfWork.DoctorProfiles.Query()
                .Include(d => d.Specializations)
                .FirstOrDefaultAsync(d => d.Id == request.DoctorProfileId, cancellationToken);

            if (doctor is null)
                return Result<bool>.Failure("Лікаря не знайдено.");

            var spec = doctor.Specializations
                .FirstOrDefault(s => s.Name.ToLower() == request.Name.ToLower());

            if (spec is null)
                return Result<bool>.Failure(
                    $"Спеціалізацію '{request.Name}' не знайдено у цього лікаря.");

            doctor.Specializations.Remove(spec);
            
            await unitOfWork.SaveChangesAsync(cancellationToken);

            return Result<bool>.Success(true);
        }
    }
}