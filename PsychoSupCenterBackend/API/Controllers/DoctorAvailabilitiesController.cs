using Application.DoctorAvailabilities.DTOs;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using PsychoSupCenterBackend.API.Controllers;
using PsychoSupCenterBackend.Application.DoctorAvailabilities.Commands;
using PsychoSupCenterBackend.Application.DoctorAvailabilities.DTOs;
using PsychoSupCenterBackend.Application.DoctorAvailabilities.Queries;

namespace PsychoSupCenterBackend.API.Controllers;

[Authorize]
public class DoctorAvailabilitiesController : BaseApiController
{
    [AllowAnonymous]
    [HttpGet("{id}")]
    public async Task<ActionResult<DoctorAvailabilityResponseDto>> GetById(Guid id)
    {
        return HandleResult(await Mediator.Send(new GetAvailabilityById.Query(id)));
    }

    [AllowAnonymous]
    [HttpGet("by-doctor/{doctorProfileId}")]
    public async Task<ActionResult<IReadOnlyList<DoctorAvailabilityResponseDto>>> GetByDoctorId(Guid doctorProfileId)
    {
        return HandleResult(await Mediator.Send(new GetAvailabilitiesByDoctorId.Query(doctorProfileId)));
    }

    [HttpPost]
    public async Task<ActionResult<DoctorAvailabilityResponseDto>> Create([FromBody] CreateDoctorAvailabilityDto dto)
    {
        return HandleResult(await Mediator.Send(new CreateDoctorAvailability.Command(dto)));
    }

    [HttpPut("{id}")]
    public async Task<ActionResult<DoctorAvailabilityResponseDto>> Update(Guid id, [FromBody] UpdateDoctorAvailabilityDto dto)
    {
        return HandleResult(await Mediator.Send(new UpdateDoctorAvailability.Command(id, dto)));
    }

    [HttpDelete("{id}")]
    public async Task<ActionResult<bool>> Delete(Guid id)
    {
        return HandleResult(await Mediator.Send(new DeleteDoctorAvailability.Command(id)));
    }
}