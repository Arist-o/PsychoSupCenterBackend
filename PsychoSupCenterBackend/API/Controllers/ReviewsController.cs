using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using PsychoSupCenterBackend.API.Controllers;
using PsychoSupCenterBackend.Application.Reviews.Commands;
using PsychoSupCenterBackend.Application.Reviews.DTOs;
using PsychoSupCenterBackend.Application.Reviews.Queries;

namespace PsychoSupCenterBackend.API.Controllers;

[Authorize]
public class ReviewsController : BaseApiController
{
    [HttpGet("{id}")]
    public async Task<ActionResult<ReviewResponseDto>> GetById(Guid id)
    {
        return HandleResult(await Mediator.Send(new GetReviewById.Query(id)));
    }

    [AllowAnonymous]
    [HttpGet("by-doctor/{doctorProfileId}")]
    public async Task<ActionResult<IReadOnlyList<ReviewResponseDto>>> GetByDoctorId(Guid doctorProfileId)
    {
        return HandleResult(await Mediator.Send(new GetReviewsByDoctorId.Query(doctorProfileId)));
    }

    [HttpGet("by-patient/{patientProfileId}")]
    public async Task<ActionResult<IReadOnlyList<ReviewResponseDto>>> GetByPatientId(Guid patientProfileId)
    {
        return HandleResult(await Mediator.Send(new GetReviewsByPatientId.Query(patientProfileId)));
    }

    [HttpPost]
    public async Task<ActionResult<ReviewResponseDto>> Create([FromBody] CreateReviewDto dto)
    {
        return HandleResult(await Mediator.Send(new CreateReview.Command(dto)));
    }

    [HttpPut("{id}")]
    public async Task<ActionResult<ReviewResponseDto>> Update(Guid id, [FromBody] UpdateReviewDto dto)
    {
        return HandleResult(await Mediator.Send(new UpdateReview.Command(id, dto)));
    }

    [HttpDelete("{id}")]
    public async Task<ActionResult<bool>> Delete(Guid id)
    {
        return HandleResult(await Mediator.Send(new DeleteReview.Command(id)));
    }
}