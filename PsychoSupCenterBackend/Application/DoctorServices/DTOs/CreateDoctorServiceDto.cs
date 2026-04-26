
namespace PsychoSupCenterBackend.Application.DoctorServices.DTOs;

public sealed record CreateDoctorServiceDto(
    string ServiceName,
    decimal Price
);