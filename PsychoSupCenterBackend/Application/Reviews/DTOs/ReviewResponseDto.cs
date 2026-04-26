namespace PsychoSupCenterBackend.Application.Reviews.DTOs;

public sealed record ReviewResponseDto(
    Guid Id,
    Guid DoctorProfileId,
    Guid PatientProfileId,
    Guid AppointmentId,
    int Rating,
    string? Comment, 
    bool IsAnonymous,
    DateTime CreatedAt
);