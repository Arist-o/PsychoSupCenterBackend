
namespace PsychoSupCenterBackend.Application.DoctorServices.DTOs;

public sealed record UpdateDoctorServiceDto(
    string ServiceName,
    decimal Price
);