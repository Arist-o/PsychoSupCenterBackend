
namespace PsychoSupCenterBackend.Application.PsychologicalTests.DTOs;

public sealed record PsychologicalTestResponseDto(
    Guid Id,
    Guid AppointmentId,
    string TestType,
    string? ResultJson,
    int ScoreTotal,
    DateTime TakenAt
);