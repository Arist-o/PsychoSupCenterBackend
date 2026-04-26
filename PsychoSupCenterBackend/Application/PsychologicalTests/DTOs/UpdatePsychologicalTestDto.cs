namespace PsychoSupCenterBackend.Application.PsychologicalTests.DTOs;

public sealed record UpdatePsychologicalTestDto(
    string? ResultJson,
    int ScoreTotal
);