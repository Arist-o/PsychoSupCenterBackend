using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using PsychoSupCenterBackend.Application.Admin.Commands;
using PsychoSupCenterBackend.Application.Admin.DTOs;
using PsychoSupCenterBackend.Application.Admin.Queries;
using PsychoSupCenterBackend.Application.Users.DTOs;

namespace PsychoSupCenterBackend.API.Controllers;

[Authorize(Roles = "Admin,HeadDoctor")] // Or however roles are managed
public class AdminController : BaseApiController
{
    [HttpGet("dashboard-stats")]
    public async Task<ActionResult<DashboardStatsDto>> GetDashboardStats()
    {
        return HandleResult(await Mediator.Send(new GetDashboardStats.Query()));
    }

    [Authorize(Roles = "Admin")]
    [HttpPost("create-admin")]
    public async Task<ActionResult<UserResponseDto>> CreateAdmin([FromBody] CreateAdminDto dto)
    {
        return HandleResult(await Mediator.Send(new CreateAdminUser.Command(dto)));
    }

    [Authorize(Roles = "Admin")]
    [HttpPut("approve-doctor/{id}")]
    public async Task<ActionResult<bool>> ApproveDoctor(Guid id)
    {
        return HandleResult(await Mediator.Send(new ApproveDoctorProfile.Command(id)));
    }
}
