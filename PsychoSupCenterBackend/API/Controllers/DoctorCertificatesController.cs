using Application.DoctorCertificates.DTOs;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using PsychoSupCenterBackend.API.Controllers;
using PsychoSupCenterBackend.Application.DoctorCertificates.Commands;
using PsychoSupCenterBackend.Application.DoctorCertificates.DTOs;
using PsychoSupCenterBackend.Application.DoctorCertificates.Queries;

namespace PsychoSupCenterBackend.API.Controllers;

[Authorize]
public class DoctorCertificatesController : BaseApiController
{
    [AllowAnonymous]
    [HttpGet("by-doctor/{doctorProfileId}")]
    public async Task<ActionResult<IReadOnlyList<DoctorCertificateResponseDto>>> GetByDoctorId(Guid doctorProfileId)
    {
        return HandleResult(await Mediator.Send(new GetCertificatesByDoctorId.Query(doctorProfileId)));
    }

    [AllowAnonymous]
    [HttpGet("{id}")]
    public async Task<ActionResult<DoctorCertificateResponseDto>> GetById(Guid id)
    {
        return HandleResult(await Mediator.Send(new GetDoctorCertificateById.Query(id)));
    }

    [HttpPost]
    public async Task<ActionResult<DoctorCertificateResponseDto>> Add([FromBody] AddDoctorCertificateDto dto)
    {
        return HandleResult(await Mediator.Send(new AddDoctorCertificate.Command(dto)));
    }

    [HttpPut("{id}")]
    public async Task<ActionResult<DoctorCertificateResponseDto>> Update(Guid id, [FromBody] UpdateDoctorCertificateDto dto)
    {
        return HandleResult(await Mediator.Send(new UpdateDoctorCertificate.Command(id, dto)));
    }

    [HttpDelete("{id}")]
    public async Task<ActionResult<bool>> Remove(Guid id)
    {
        return HandleResult(await Mediator.Send(new RemoveDoctorCertificate.Command(id)));
    }
}