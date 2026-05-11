using MediatR;
using Microsoft.EntityFrameworkCore;
using PsychoSupCenterBackend.Application.Common.Behaviors;
using PsychoSupCenterBackend.Application.Common.Interfaces;
using PsychoSupCenterBackend.Application.Common.Models;
using PsychoSupCenterBackend.Domain.Enums;

namespace PsychoSupCenterBackend.Application.DoctorAvailabilities.Queries;

public class GetAvailableSlots
{
    public sealed record Query(Guid DoctorProfileId, DateTime Date, int DurationMinutes = 60) : IQuery<Result<List<TimeSpan>>>;

    public sealed class Handler : IRequestHandler<Query, Result<List<TimeSpan>>>
    {
        private readonly IUnitOfWork _unitOfWork;

        public Handler(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task<Result<List<TimeSpan>>> Handle(Query request, CancellationToken cancellationToken)
        {
            var date = request.Date.Date;
            var dayOfWeek = date.DayOfWeek;

            var availabilities = await _unitOfWork.DoctorAvailabilities
                .FindAsync(x => x.DoctorProfileId == request.DoctorProfileId && x.Day == dayOfWeek, cancellationToken);

            if (!availabilities.Any())
            {
                return Result<List<TimeSpan>>.Success(new List<TimeSpan>());
            }

            var unavailabilities = await _unitOfWork.DoctorUnavailabilities
                .FindAsync(x => x.DoctorProfileId == request.DoctorProfileId && x.StartDate <= date.AddDays(1) && x.EndDate >= date, cancellationToken);

            var appointments = await _unitOfWork.Appointments
                .FindAsync(x => x.DoctorProfileId == request.DoctorProfileId 
                            && x.ScheduledAt.Date == date 
                            && x.Status != AppointmentStatus.Cancelled, cancellationToken);

            var availableSlots = new List<TimeSpan>();
            var durationSpan = TimeSpan.FromMinutes(request.DurationMinutes);

            foreach (var availability in availabilities)
            {
                var currentSlotStart = availability.StartTime;

                while (currentSlotStart.Add(durationSpan) <= availability.EndTime)
                {
                    var slotEnd = currentSlotStart.Add(durationSpan);
                    var slotStartDateTime = date.Add(currentSlotStart);
                    var slotEndDateTime = date.Add(slotEnd);

                    bool isOverlapWithAppointment = appointments.Any(a => 
                        a.ScheduledAt < slotEndDateTime && 
                        a.ScheduledAt.AddMinutes(a.DurationMinutes) > slotStartDateTime);

                    bool isOverlapWithUnavailability = unavailabilities.Any(u => 
                        u.StartDate < slotEndDateTime && 
                        u.EndDate > slotStartDateTime);

                    if (!isOverlapWithAppointment && !isOverlapWithUnavailability)
                    {
                        availableSlots.Add(currentSlotStart);
                    }

                    currentSlotStart = currentSlotStart.Add(durationSpan);
                }
            }

            return Result<List<TimeSpan>>.Success(availableSlots.OrderBy(s => s).ToList());
        }
    }
}
