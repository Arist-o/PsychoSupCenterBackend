using FluentValidation;
using MediatR;
using Microsoft.EntityFrameworkCore;
using PsychoSupCenterBackend.Application.Common.Behaviors;
using PsychoSupCenterBackend.Application.Common.Interfaces;
using PsychoSupCenterBackend.Application.Common.Models;
using PsychoSupCenterBackend.Application.Doctors.DTOs;

namespace PsychoSupCenterBackend.Application.Doctors.Commands;

public static class AssignDoctorSpecializations
{
    public sealed record Command(Guid DoctorProfileId, AssignSpecializationsDto Dto) : ICommand<Result<bool>>;

    public sealed class Validator : AbstractValidator<Command>
    {
        public Validator()
        {
            RuleFor(x => x.DoctorProfileId).NotEmpty();
            RuleFor(x => x.Dto.SpecializationIds).NotNull();
        }
    }

    public sealed class Handler(IUnitOfWork unitOfWork) : IRequestHandler<Command, Result<bool>>
    {
        public async Task<Result<bool>> Handle(Command request, CancellationToken cancellationToken)
        {
            var doctor = await unitOfWork.DoctorProfiles.Query(asNoTracking: false)
                .Include(d => d.Specializations)
                .FirstOrDefaultAsync(d => d.Id == request.DoctorProfileId, cancellationToken);

            if (doctor is null) return Result<bool>.Failure("Профіль лікаря не знайдено.");

            var specializations = await unitOfWork.DoctorSpecializations.Query(asNoTracking: false)
                .Where(s => request.Dto.SpecializationIds.Contains(s.Id))
                .ToListAsync(cancellationToken);

            doctor.Specializations.Clear();
            foreach (var spec in specializations)
            {
                doctor.Specializations.Add(spec);
            }

            doctor.UpdatedAt = DateTime.UtcNow;

            await unitOfWork.SaveChangesAsync(cancellationToken);

            return Result<bool>.Success(true);
        }
    }
}