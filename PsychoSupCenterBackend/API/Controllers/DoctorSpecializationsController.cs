using Application.DoctorSpecializations.DTOs;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using PsychoSupCenterBackend.API.Controllers;
using PsychoSupCenterBackend.Application.DoctorSpecializations.Commands;
using PsychoSupCenterBackend.Application.DoctorSpecializations.DTOs;
using PsychoSupCenterBackend.Application.DoctorSpecializations.Queries;

namespace PsychoSupCenterBackend.API.Controllers;

[Authorize]
public class DoctorSpecializationsController : BaseApiController
{
    [AllowAnonymous]
    [HttpGet]
    public async Task<ActionResult<IReadOnlyList<SpecializationResponseDto>>> GetAll()
    {
        return HandleResult(await Mediator.Send(new GetAllSpecializations.Query()));
    }

    [AllowAnonymous]
    [HttpGet("{id}")]
    public async Task<ActionResult<SpecializationResponseDto>> GetById(Guid id)
    {
        return HandleResult(await Mediator.Send(new GetSpecializationById.Query(id)));
    }

    [Authorize(Roles = "Admin")]
    [HttpPost]
    public async Task<ActionResult<SpecializationResponseDto>> Create([FromBody] CreateSpecializationDto dto)
    {
        return HandleResult(await Mediator.Send(new CreateSpecialization.Command(dto)));
    }

    [Authorize(Roles = "Admin")]
    [HttpDelete("{id}")]
    public async Task<ActionResult<bool>> Delete(Guid id)
    {
        return HandleResult(await Mediator.Send(new DeleteSpecialization.Command(id)));
    }

    [Authorize(Roles = "Admin,Doctor")]
    [HttpPost("assign")]
    public async Task<ActionResult<bool>> AssignToDoctor([FromBody] AssignSpecializationDto dto)
    {
        return HandleResult(await Mediator.Send(new AssignSpecializationToDoctor.Command(dto)));
    }

    [Authorize(Roles = "Admin")]
    [HttpPut("{id}")]
    public async Task<ActionResult<SpecializationResponseDto>> Update(Guid id, [FromBody] UpdateSpecializationDto dto)
    {
        return HandleResult(await Mediator.Send(new UpdateSpecialization.Command(id, dto)));
    }

    [Authorize(Roles = "Admin,Doctor")]
    [HttpDelete("unassign")]
    public async Task<ActionResult<bool>> UnassignSpecialization([FromBody] RemoveSpecializationDto dto)
    {
        return HandleResult(await Mediator.Send(new RemoveSpecializationFromDoctor.Command(dto.DoctorProfileId, dto.Name)));
    }
}