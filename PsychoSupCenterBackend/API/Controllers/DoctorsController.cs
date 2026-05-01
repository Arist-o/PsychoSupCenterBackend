using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using PsychoSupCenterBackend.Application.Doctors.Commands;
using PsychoSupCenterBackend.Application.Doctors.Queries;
using PsychoSupCenterBackend.Application.Doctors.DTOs;

namespace PsychoSupCenterBackend.API.Controllers;

public class DoctorsController : BaseApiController
{
    [AllowAnonymous]
    [HttpGet]
    public async Task<ActionResult<IReadOnlyList<DoctorProfileResponseDto>>> GetAll([FromQuery] GetAllDoctors.Query query)
    {
        return HandleResult(await Mediator.Send(query));
    }

    [AllowAnonymous]
    [HttpGet("{id}")]
    public async Task<ActionResult<DoctorProfileResponseDto>> GetById(Guid id)
    {
        return HandleResult(await Mediator.Send(new GetDoctorById.Query(id)));
    }

    [AllowAnonymous]
    [HttpGet("by-specialization")]
    public async Task<ActionResult<IReadOnlyList<DoctorProfileResponseDto>>> GetBySpecialization([FromQuery] GetDoctorsBySpecialization.Query query)
    {
        return HandleResult(await Mediator.Send(query));
    }

    [Authorize(Roles = "Admin,Doctor")]
    [HttpPost("{userId}")]
    public async Task<ActionResult<DoctorProfileResponseDto>> Create(Guid userId, [FromBody] CreateDoctorProfileDto dto)
    {
        return HandleResult(await Mediator.Send(new CreateDoctorProfile.Command(userId, dto)));
    }

    [Authorize(Roles = "Admin,Doctor")]
    [HttpPut("{id}")]
    public async Task<ActionResult<DoctorProfileResponseDto>> Update(Guid id, [FromBody] UpdateDoctorProfileDto dto)
    {
        return HandleResult(await Mediator.Send(new UpdateDoctorProfile.Command(id, dto)));
    }

    [Authorize(Roles = "Admin,Doctor")]
    [HttpPatch("{id}/status")]
    public async Task<ActionResult<DoctorProfileResponseDto>> ChangeStatus(Guid id, [FromBody] ChangeDoctorStatusDto dto)
    {
        return HandleResult(await Mediator.Send(new ChangeDoctorStatus.Command(id, dto)));
    }

    [Authorize(Roles = "Admin")]
    [HttpDelete("{id}")]
    public async Task<ActionResult<bool>> Delete(Guid id)
    {
        return HandleResult(await Mediator.Send(new DeleteDoctorProfile.Command(id)));
    }
}