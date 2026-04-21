using PsychoSupCenterBackend.Domain.Common;

namespace PsychoSupCenterBackend.Domain.Entities;

public class DoctorCertificate : BaseEntity
{
    public Guid DoctorProfileId { get; set; }
    public string CertificateUrl { get; set; } = string.Empty;
    public DateTime AddedAt { get; set; } = DateTime.UtcNow;

    public DoctorProfile DoctorProfile { get; set; } = null!;
}