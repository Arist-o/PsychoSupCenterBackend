using PsychoSupCenterBackend.Domain.Common;

namespace PsychoSupCenterBackend.Domain.Entities;

public class DoctorCertificate : BaseEntity
{
    public Guid DoctorProfileId { get; set; }
    public string Name { get; set; } = string.Empty;
    public string IssuingOrganization { get; set; } = string.Empty;
    public DateTime IssueDate { get; set; }
    public string CertificateUrl { get; set; } = string.Empty;
    public DateTime AddedAt { get; set; }

    public DoctorProfile DoctorProfile { get; set; } = null!;
}