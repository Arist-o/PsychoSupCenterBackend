using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using PsychoSupCenterBackend.Application.Patients.Commands;
using PsychoSupCenterBackend.Application.Patients.Queries;
using PsychoSupCenterBackend.Application.Patients.DTOs;
using Application.Patients.Queries;

namespace PsychoSupCenterBackend.API.Controllers;

[Authorize] 
public class PatientsController : BaseApiController
{
    [Authorize(Roles = "Admin,Doctor")]
    [HttpGet]
    public async Task<ActionResult<IReadOnlyList<PatientProfileResponseDto>>> GetAll([FromQuery] GetAllPatients.Query query)
    {
        return HandleResult(await Mediator.Send(query));
    }

    [HttpGet("{id}")]
    public async Task<ActionResult<PatientProfileResponseDto>> GetById(Guid id)
    {
        return HandleResult(await Mediator.Send(new GetPatientById.Query(id)));
    }

    [HttpPost("{userId}")]
    public async Task<ActionResult<PatientProfileResponseDto>> Create(Guid userId, [FromBody] CreatePatientProfileDto dto)
    {
        return HandleResult(await Mediator.Send(new CreatePatientProfile.Command(userId, dto)));
    }

    [HttpPut("{id}")]
    public async Task<ActionResult<PatientProfileResponseDto>> Update(Guid id, [FromBody] UpdatePatientProfileDto dto)
    {
        return HandleResult(await Mediator.Send(new UpdatePatientProfile.Command(id, dto)));
    }

    [Authorize(Roles = "Admin")]
    [HttpDelete("{id}")]
    public async Task<ActionResult<bool>> Delete(Guid id)
    {
        return HandleResult(await Mediator.Send(new DeletePatientProfile.Command(id)));
    }
}