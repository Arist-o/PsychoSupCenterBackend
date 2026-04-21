using PsychoSupCenterBackend.Domain.Common;

namespace PsychoSupCenterBackend.Domain.Entities;

public class DoctorService : BaseEntity
{
    public Guid DoctorProfileId { get; set; }
    public string ServiceName { get; set; } = string.Empty;
    public decimal Price { get; set; }

    public DoctorProfile DoctorProfile { get; set; } = null!;
    public ICollection<Appointment> Appointments { get; set; } = [];
    public ICollection<Billing> Billings { get; set; } = [];
}