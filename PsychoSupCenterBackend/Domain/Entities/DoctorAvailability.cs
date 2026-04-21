using PsychoSupCenterBackend.Domain.Common;

namespace PsychoSupCenterBackend.Domain.Entities;

public class DoctorAvailability : BaseEntity
{
    public Guid DoctorProfileId { get; set; }
    public DayOfWeek Day { get; set; }
    public TimeSpan StartTime { get; set; }
    public TimeSpan EndTime { get; set; }

    public DoctorProfile DoctorProfile { get; set; } = null!;
}