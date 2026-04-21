using PsychoSupCenterBackend.Domain.Common;

namespace PsychoSupCenterBackend.Domain.Entities;

public class DoctorSpecialization : BaseEntity
{
    public Guid DoctorProfileId { get; set; }
    public string Name { get; set; } = string.Empty;

    public DoctorProfile DoctorProfile { get; set; } = null!;
}