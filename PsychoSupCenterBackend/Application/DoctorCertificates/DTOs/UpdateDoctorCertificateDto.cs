
namespace Application.DoctorCertificates.DTOs;

public sealed record UpdateDoctorCertificateDto(
    string Name,
    string IssuingOrganization,
    DateTime IssueDate,
    string CertificateUrl
);