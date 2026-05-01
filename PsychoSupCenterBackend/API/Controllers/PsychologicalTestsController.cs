using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using PsychoSupCenterBackend.API.Controllers;
using PsychoSupCenterBackend.Application.PsychologicalTests.Commands;
using PsychoSupCenterBackend.Application.PsychologicalTests.DTOs;
using PsychoSupCenterBackend.Application.PsychologicalTests.Queries;

namespace PsychoSupCenterBackend.API.Controllers;

[Authorize]
public class PsychologicalTestsController : BaseApiController
{
    [HttpGet]
    public async Task<ActionResult<IReadOnlyList<PsychologicalTestResponseDto>>> GetAll([FromQuery] int page = 1, [FromQuery] int pageSize = 20)
    {
        return HandleResult(await Mediator.Send(new GetAllPsychologicalTests.Query(page, pageSize)));
    }

    [HttpGet("{id}")]
    public async Task<ActionResult<PsychologicalTestResponseDto>> GetById(Guid id)
    {
        return HandleResult(await Mediator.Send(new GetPsychologicalTestById.Query(id)));
    }

    [HttpGet("by-patient/{patientProfileId}")]
    public async Task<ActionResult<IReadOnlyList<PsychologicalTestResponseDto>>> GetByPatientId(Guid patientProfileId)
    {
        return HandleResult(await Mediator.Send(new GetTestsByPatientId.Query(patientProfileId)));
    }

    [HttpPost]
    public async Task<ActionResult<PsychologicalTestResponseDto>> Create([FromBody] CreatePsychologicalTestDto dto)
    {
        return HandleResult(await Mediator.Send(new CreatePsychologicalTest.Command(dto)));
    }

    [HttpPut("{id}")]
    public async Task<ActionResult<PsychologicalTestResponseDto>> Update(Guid id, [FromBody] UpdatePsychologicalTestDto dto)
    {
        return HandleResult(await Mediator.Send(new UpdatePsychologicalTest.Command(id, dto)));
    }

    [HttpDelete("{id}")]
    public async Task<ActionResult<bool>> Delete(Guid id)
    {
        return HandleResult(await Mediator.Send(new DeletePsychologicalTest.Command(id)));
    }
}