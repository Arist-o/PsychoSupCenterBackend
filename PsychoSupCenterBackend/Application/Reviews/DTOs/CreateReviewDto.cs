
namespace PsychoSupCenterBackend.Application.Reviews.DTOs;

public sealed record CreateReviewDto(
    Guid DoctorProfileId,
    Guid PatientProfileId,
    Guid AppointmentId,
    int Rating,
    string? Comment,
    bool IsAnonymous
);