// API/Controllers/PatientsController.cs
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using PsychoSupCenterBackend.Application.Patients.Commands;
using PsychoSupCenterBackend.Application.Patients.DTOs;
using PsychoSupCenterBackend.Application.Patients.Queries;

namespace API.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class PatientsController(ISender sender) : ControllerBase
{
    // ════════════════════════════════════════════════════════════
    // PATIENT PROFILE CRUD
    // ════════════════════════════════════════════════════════════

    /// <summary>GET /api/patients — Список всіх пацієнтів (Admin only) з пагінацією</summary>
    [HttpGet]
    [ProducesResponseType(typeof(IReadOnlyList<PatientProfileResponseDto>), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetAll(
        [FromQuery] int page = 1,
        [FromQuery] int pageSize = 20,
        CancellationToken cancellationToken = default)
    {
        var result = await sender.Send(
            new GetAllPatients.Query(page, pageSize),
            cancellationToken);

        return result.IsSuccess
            ? Ok(result.Value)
            : BadRequest(new { error = result.Error });
    }

    /// <summary>GET /api/patients/{patientProfileId} — Профіль пацієнта за Id</summary>
    [HttpGet("{patientProfileId:guid}")]
    [ProducesResponseType(typeof(PatientProfileResponseDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetById(
        [FromRoute] Guid patientProfileId,
        CancellationToken cancellationToken)
    {
        var result = await sender.Send(
            new GetPatientProfileById.Query(patientProfileId),
            cancellationToken);

        return result.IsSuccess
            ? Ok(result.Value)
            : NotFound(new { error = result.Error });
    }

    /// <summary>
    /// POST /api/patients — Створити профіль пацієнта для існуючого User.
    /// UserId передається як query parameter (береться з JWT claims у реальному сценарії).
    /// </summary>
    [HttpPost]
    [ProducesResponseType(typeof(PatientProfileResponseDto), StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> Create(
        [FromQuery] Guid userId,
        [FromBody] CreatePatientProfileDto dto,
        CancellationToken cancellationToken)
    {
        var result = await sender.Send(
            new CreatePatientProfile.Command(userId, dto),
            cancellationToken);

        return result.IsSuccess
            ? CreatedAtAction(nameof(GetById),
                new { patientProfileId = result.Value!.Id }, result.Value)
            : BadRequest(new { error = result.Error });
    }

    /// <summary>PUT /api/patients/{patientProfileId} — Оновити дані профілю пацієнта</summary>
    [HttpPut("{patientProfileId:guid}")]
    [ProducesResponseType(typeof(PatientProfileResponseDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> Update(
        [FromRoute] Guid patientProfileId,
        [FromBody] UpdatePatientProfileDto dto,
        CancellationToken cancellationToken)
    {
        var result = await sender.Send(
            new UpdatePatientProfile.Command(patientProfileId, dto),
            cancellationToken);

        return result.IsSuccess
            ? Ok(result.Value)
            : BadRequest(new { error = result.Error });
    }

    /// <summary>DELETE /api/patients/{patientProfileId} — Видалити профіль пацієнта</summary>
    [HttpDelete("{patientProfileId:guid}")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> Delete(
        [FromRoute] Guid patientProfileId,
        CancellationToken cancellationToken)
    {
        var result = await sender.Send(
            new DeletePatientProfile.Command(patientProfileId),
            cancellationToken);

        return result.IsSuccess
            ? Ok(new { message = "Профіль пацієнта видалено." })
            : BadRequest(new { error = result.Error });
    }
}