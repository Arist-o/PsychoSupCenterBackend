
namespace PsychoSupCenterBackend.Application.DoctorCertificates.DTOs;

public sealed record DoctorCertificateResponseDto(
    Guid Id,
    Guid DoctorProfileId,
    string Name,
    string IssuingOrganization,
    DateTime IssueDate,
    string CertificateUrl,
    DateTime AddedAt
);