namespace PsychoSupCenterBackend.Application.PsychologicalTests.DTOs;

public sealed record CreatePsychologicalTestDto(
    Guid AppointmentId,
    string TestType,
    string? ResultJson,
    int ScoreTotal
);