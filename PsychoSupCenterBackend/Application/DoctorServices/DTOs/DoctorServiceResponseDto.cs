
namespace PsychoSupCenterBackend.Application.DoctorServices.DTOs;

public sealed record DoctorServiceResponseDto(
    Guid Id,
    Guid DoctorProfileId,
    string ServiceName,
    decimal Price
);