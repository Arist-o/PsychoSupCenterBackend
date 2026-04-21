using PsychoSupCenterBackend.Domain.Common;

namespace PsychoSupCenterBackend.Domain.Entities;

public class Review : BaseEntity
{
    public Guid DoctorProfileId { get; set; }
    public Guid PatientProfileId { get; set; }
    public Guid AppointmentId { get; set; }
    public int Rating { get; set; }
    public string? Comment { get; set; }
    public bool IsAnonymous { get; set; } = false;
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    public DoctorProfile DoctorProfile { get; set; } = null!;
    public PatientProfile PatientProfile { get; set; } = null!;
    public Appointment Appointment { get; set; } = null!;
}