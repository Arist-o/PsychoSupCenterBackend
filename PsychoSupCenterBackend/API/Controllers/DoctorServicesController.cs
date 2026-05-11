using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using PsychoSupCenterBackend.API.Controllers;
using PsychoSupCenterBackend.Application.DoctorServices.Commands;
using PsychoSupCenterBackend.Application.DoctorServices.DTOs;
using PsychoSupCenterBackend.Application.DoctorServices.Queries;

namespace PsychoSupCenterBackend.API.Controllers;

[Authorize]
public class DoctorServicesController : BaseApiController
{
    [AllowAnonymous]
    [HttpGet("active")]
    public async Task<ActionResult<IReadOnlyList<DoctorServiceResponseDto>>> GetAllActive([FromQuery] int page = 1, [FromQuery] int pageSize = 50)
    {
        return HandleResult(await Mediator.Send(new GetAllActiveServices.Query(page, pageSize)));
    }

    [AllowAnonymous]
    [HttpGet("{id:guid}")]
    public async Task<ActionResult<DoctorServiceResponseDto>> GetById(Guid id)
    {
        return HandleResult(await Mediator.Send(new GetDoctorServiceById.Query(id)));
    }

    [AllowAnonymous]
    [HttpGet("by-doctor/{doctorProfileId:guid}")]
    public async Task<ActionResult<IReadOnlyList<DoctorServiceResponseDto>>> GetByDoctorId(Guid doctorProfileId)
    {
        return HandleResult(await Mediator.Send(new GetServicesByDoctorId.Query(doctorProfileId)));
    }

    [HttpPost]
    public async Task<ActionResult<DoctorServiceResponseDto>> Create([FromQuery] Guid doctorProfileId, [FromBody] CreateDoctorServiceDto dto)
    {
        return HandleResult(await Mediator.Send(new CreateDoctorService.Command(doctorProfileId, dto)));
    }

    [HttpPut("{id:guid}")]
    public async Task<ActionResult<DoctorServiceResponseDto>> Update(Guid id, [FromBody] UpdateDoctorServiceDto dto)
    {
        return HandleResult(await Mediator.Send(new UpdateDoctorService.Command(id, dto)));
    }

    [HttpDelete("{id:guid}")]
    public async Task<ActionResult<bool>> Delete(Guid id)
    {
        return HandleResult(await Mediator.Send(new DeleteDoctorService.Command(id)));
    }
}