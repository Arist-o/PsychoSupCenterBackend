using PsychoSupCenterBackend.Domain.Common;

namespace PsychoSupCenterBackend.Domain.Entities;

public class DoctorUnavailability : BaseEntity
{
    public Guid DoctorProfileId { get; set; }
    public DateTime StartDate { get; set; }
    public DateTime EndDate { get; set; }
    public string? Reason { get; set; }

    public DoctorProfile DoctorProfile { get; set; } = null!;
}