
using PsychoSupCenterBackend.Domain.Common;
using PsychoSupCenterBackend.Domain.Enums;

namespace PsychoSupCenterBackend.Domain.Entities;

public class DoctorProfile : BaseEntity
{
    public Guid UserId { get; set; }
    public string? Bio { get; set; }
    public DateTime CareerStartDate { get; set; }
    public DoctorStatus Status { get; set; } = DoctorStatus.Active;
    public double AverageRating { get; set; } = 0.0;
    public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;

    public ApplicationUser User { get; set; } = null!;

    public ICollection<DoctorCertificate> Certificates { get; set; } = [];
    public ICollection<DoctorService> Services { get; set; } = [];
    public ICollection<DoctorAvailability> Availabilities { get; set; } = [];
    public ICollection<DoctorUnavailability> Unavailabilities { get; set; } = [];
    public ICollection<DoctorSpecialization> Specializations { get; set; } = [];
    public ICollection<Appointment> Appointments { get; set; } = [];
    public ICollection<Review> Reviews { get; set; } = [];

    public int ExperienceYears =>
        (int)((DateTime.UtcNow - CareerStartDate).TotalDays / 365.25);
}