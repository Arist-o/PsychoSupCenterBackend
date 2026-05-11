
namespace PsychoSupCenterBackend.Application.DoctorServices.DTOs;

public sealed record CreateDoctorServiceDto(
    string ServiceName,
    decimal Price,
    string? Description = null,
    int DurationMinutes = 60
);