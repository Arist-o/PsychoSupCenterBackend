using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using PsychoSupCenterBackend.Application.Admin.DTOs;
using PsychoSupCenterBackend.Application.Admin.Queries;

namespace PsychoSupCenterBackend.API.Controllers;

[Authorize(Roles = "Admin,HeadDoctor")] // Or however roles are managed
public class AdminController : BaseApiController
{
    [HttpGet("dashboard-stats")]
    public async Task<ActionResult<DashboardStatsDto>> GetDashboardStats()
    {
        return HandleResult(await Mediator.Send(new GetDashboardStats.Query()));
    }
}
