using FluentValidation;
using MediatR;
using PsychoSupCenterBackend.Application.Common.Behaviors;
using PsychoSupCenterBackend.Application.Common.Interfaces;
using PsychoSupCenterBackend.Application.Common.Models;
using PsychoSupCenterBackend.Application.Appointments.DTOs;

namespace PsychoSupCenterBackend.Application.Appointments.Queries;

public static class GetAppointmentsByDoctorId
{
    public sealed record Query(Guid DoctorProfileId) : IQuery<Result<IReadOnlyList<AppointmentResponseDto>>>;

    public sealed class Validator : AbstractValidator<Query>
    {
        public Validator() => RuleFor(x => x.DoctorProfileId).NotEmpty();
    }

    public sealed class Handler(IUnitOfWork unitOfWork) : IRequestHandler<Query, Result<IReadOnlyList<AppointmentResponseDto>>>
    {
        public async Task<Result<IReadOnlyList<AppointmentResponseDto>>> Handle(Query request, CancellationToken cancellationToken)
        {
            var appointments = await unitOfWork.Appointments.FindAsync(a => a.DoctorProfileId == request.DoctorProfileId, cancellationToken);

            var result = appointments.Select(appt => new AppointmentResponseDto(
                appt.Id, appt.DoctorProfileId, appt.PatientProfileId, appt.DoctorServiceId,
                appt.ChatRoomId, appt.BillingId, appt.ScheduledAt, appt.DurationMinutes,
                appt.Status, appt.Type, appt.Notes, appt.CreatedAt)).ToList();

            return Result<IReadOnlyList<AppointmentResponseDto>>.Success(result);
        }
    }
}