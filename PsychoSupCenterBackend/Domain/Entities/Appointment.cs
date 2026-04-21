
using PsychoSupCenterBackend.Domain.Common;
using PsychoSupCenterBackend.Domain.Enums;

namespace PsychoSupCenterBackend.Domain.Entities;

public class Appointment : BaseEntity
{
    public Guid DoctorProfileId { get; set; }
    public Guid PatientProfileId { get; set; }
    public Guid DoctorServiceId { get; set; }
    public Guid? ChatRoomId { get; set; }
    public Guid? BillingId { get; set; }
    public DateTime ScheduledAt { get; set; }
    public int DurationMinutes { get; set; }
    public AppointmentStatus Status { get; set; } = AppointmentStatus.Scheduled;
    public string? Type { get; set; }
    public string? Notes { get; set; }
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    public DoctorProfile DoctorProfile { get; set; } = null!;
    public PatientProfile PatientProfile { get; set; } = null!;
    public DoctorService DoctorService { get; set; } = null!;
    public ChatRoom? ChatRoom { get; set; }
    public Billing? Billing { get; set; }
    public Review? Review { get; set; }
    public PsychologicalTest? PsychologicalTest { get; set; }
}