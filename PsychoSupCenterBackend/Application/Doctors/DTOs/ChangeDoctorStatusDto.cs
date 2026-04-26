using PsychoSupCenterBackend.Domain.Enums;
namespace PsychoSupCenterBackend.Application.Doctors.DTOs;

public sealed record ChangeDoctorStatusDto(DoctorStatus NewStatus);