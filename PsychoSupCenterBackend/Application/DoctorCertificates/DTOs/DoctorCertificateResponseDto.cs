
namespace PsychoSupCenterBackend.Application.DoctorCertificates.DTOs;

public sealed record DoctorCertificateResponseDto(
    Guid Id,
    Guid DoctorProfileId,
    string CertificateUrl,
    DateTime AddedAt
);