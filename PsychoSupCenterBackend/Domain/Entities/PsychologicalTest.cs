using PsychoSupCenterBackend.Domain.Common;

namespace PsychoSupCenterBackend.Domain.Entities;

public class PsychologicalTest : BaseEntity
{
    public Guid AppointmentId { get; set; }
    public string TestType { get; set; } = string.Empty;
    public string? ResultJson { get; set; }
    public int ScoreTotal { get; set; }
    public DateTime TakenAt { get; set; } = DateTime.UtcNow;

    public Appointment Appointment { get; set; } = null!;
}