
namespace PsychoSupCenterBackend.Application.DoctorCertificates.DTOs;

public sealed record AddDoctorCertificateDto(
    Guid DoctorProfileId,
    string CertificateUrl
);