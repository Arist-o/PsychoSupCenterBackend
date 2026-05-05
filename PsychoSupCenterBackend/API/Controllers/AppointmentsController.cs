using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using PsychoSupCenterBackend.API.Controllers;
using PsychoSupCenterBackend.Application.Appointments.Commands;
using PsychoSupCenterBackend.Application.Appointments.DTOs;
using PsychoSupCenterBackend.Application.Appointments.Queries;

namespace PsychoSupCenterBackend.API.Controllers;

[Authorize]
public class AppointmentsController : BaseApiController
{
    [HttpGet("{id}")]
    public async Task<ActionResult<AppointmentResponseDto>> GetById(Guid id)
    {
        return HandleResult(await Mediator.Send(new GetAppointmentById.Query(id)));
    }

    [HttpGet("by-doctor/{doctorProfileId}")]
    public async Task<ActionResult<IReadOnlyList<AppointmentResponseDto>>> GetByDoctorId(Guid doctorProfileId, [FromQuery] int page = 1, [FromQuery] int pageSize = 20)
    {
        return HandleResult(await Mediator.Send(new GetAppointmentsByDoctorId.Query(doctorProfileId, page, pageSize)));
    }

    [HttpGet("by-patient/{patientProfileId}")]
    public async Task<ActionResult<IReadOnlyList<AppointmentResponseDto>>> GetByPatientId(Guid patientProfileId, [FromQuery] int page = 1, [FromQuery] int pageSize = 20)
    {
        return HandleResult(await Mediator.Send(new GetAppointmentsByPatientId.Query(patientProfileId, page, pageSize)));
    }

    [HttpPost]
    public async Task<ActionResult<AppointmentResponseDto>> Create([FromBody] CreateAppointmentDto dto)
    {
        return HandleResult(await Mediator.Send(new CreateAppointment.Command(dto)));
    }

    [HttpPut("{id}")]
    public async Task<ActionResult<AppointmentResponseDto>> Update(Guid id, [FromBody] UpdateAppointmentDto dto)
    {
        return HandleResult(await Mediator.Send(new UpdateAppointment.Command(id, dto)));
    }

    [HttpDelete("{id}")]
    public async Task<ActionResult<bool>> Delete(Guid id)
    {
        return HandleResult(await Mediator.Send(new DeleteAppointment.Command(id)));
    }

    [HttpPatch("{id}/cancel")]
    public async Task<ActionResult<bool>> Cancel(Guid id)
    {
        return HandleResult(await Mediator.Send(new CancelAppointment.Command(id)));
    }
}