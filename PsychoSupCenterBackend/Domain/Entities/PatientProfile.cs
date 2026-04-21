using PsychoSupCenterBackend.Domain.Common;
using PsychoSupCenterBackend.Domain.Enums;

namespace PsychoSupCenterBackend.Domain.Entities;

public class PatientProfile : BaseEntity
{
    public Guid UserId { get; set; }
    public PatientType Type { get; set; } = PatientType.Standard;
    public string? MilitaryId { get; set; }
    public string? EmergencyContact { get; set; }
    public DateTime DateOfBirth { get; set; }

    public ApplicationUser User { get; set; } = null!;
    public ICollection<Appointment> Appointments { get; set; } = [];
    public ICollection<Review> Reviews { get; set; } = [];
}