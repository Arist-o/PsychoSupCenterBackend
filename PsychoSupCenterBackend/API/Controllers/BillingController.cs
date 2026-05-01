using Application.Billing.DTOs;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using PsychoSupCenterBackend.API.Controllers;
using PsychoSupCenterBackend.Application.Billing.Commands;
using PsychoSupCenterBackend.Application.Billing.DTOs;
using PsychoSupCenterBackend.Application.Billing.Queries;
using PsychoSupCenterBackend.Domain.Enums;

namespace PsychoSupCenterBackend.API.Controllers;

[Authorize]
public class BillingController : BaseApiController
{
    [HttpGet("{id:guid}")]
    public async Task<ActionResult<BillingResponseDto>> GetById(Guid id)
    {
        return HandleResult(await Mediator.Send(new GetBillingById.Query(id)));
    }

    [HttpGet("by-appointment/{appointmentId:guid}")]
    public async Task<ActionResult<BillingResponseDto>> GetByAppointmentId(Guid appointmentId)
    {
        return HandleResult(await Mediator.Send(new GetBillingsByAppointmentId.Query(appointmentId)));
    }

    [HttpGet("by-patient/{patientProfileId:guid}")]
    public async Task<ActionResult<IReadOnlyList<BillingResponseDto>>> GetByPatientId(Guid patientProfileId)
    {
        return HandleResult(await Mediator.Send(new GetBillingsByPatientId.Query(patientProfileId)));
    }

    [HttpPut("{id:guid}/status")]
    public async Task<ActionResult<BillingResponseDto>> UpdatePaymentStatus(Guid id, [FromBody] UpdatePaymentStatusDto dto)
    {
        return HandleResult(await Mediator.Send(new UpdatePaymentStatus.Command(id, dto.NewStatus)));
    }

    [HttpPost("{id:guid}/refund")]
    public async Task<ActionResult<BillingResponseDto>> Refund(Guid id)
    {
        return HandleResult(await Mediator.Send(new RefundBilling.Command(id)));
    }
}

