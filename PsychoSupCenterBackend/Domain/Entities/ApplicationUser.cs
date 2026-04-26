using PsychoSupCenterBackend.Domain.Common;
using PsychoSupCenterBackend.Domain.Enums;

namespace PsychoSupCenterBackend.Domain.Entities;

public class ApplicationUser : BaseEntity
{
    public string Email { get; set; } = string.Empty;
    public string FirstName { get; set; } = string.Empty;
    public string LastName { get; set; } = string.Empty;
    public string? PhoneNumber { get; set; }
    public string? PhotoUrl { get; set; }

    public string PasswordHash { get; set; } = string.Empty;

    public UserRole Role { get; set; }
    public bool IsActive { get; set; } = true;
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;

    public DoctorProfile? DoctorProfile { get; set; }
    public PatientProfile? PatientProfile { get; set; }

    public ICollection<ChatParticipant> ChatParticipants { get; set; } = [];
    public ICollection<ChatMessage> SentMessages { get; set; } = [];
}