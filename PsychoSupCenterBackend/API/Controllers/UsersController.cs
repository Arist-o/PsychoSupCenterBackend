using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using PsychoSupCenterBackend.Application.Users.Commands;
using PsychoSupCenterBackend.Application.Users.Queries;
using PsychoSupCenterBackend.Application.Users.DTOs;

namespace PsychoSupCenterBackend.API.Controllers;

public class UsersController : BaseApiController
{
    [AllowAnonymous]
    [HttpPost("register")]
    public async Task<ActionResult<AuthResponseDto>> Register([FromBody] RegisterDto dto)
    {
        var ipAddress = HttpContext.Connection.RemoteIpAddress?.ToString();
        return HandleResult(await Mediator.Send(new RegisterUser.Command(dto, ipAddress)));
    }

    [AllowAnonymous]
    [HttpPost("login")]
    public async Task<ActionResult<AuthResponseDto>> Login([FromBody] LoginDto dto)
    {
        var ipAddress = HttpContext.Connection.RemoteIpAddress?.ToString();
        return HandleResult(await Mediator.Send(new LoginUser.Command(dto, ipAddress)));
    }

    [AllowAnonymous]
    [HttpPost("refresh-token")]
    public async Task<ActionResult<AuthResponseDto>> RefreshToken([FromBody] RefreshTokenCommand.Command command)
    {
        return HandleResult(await Mediator.Send(command));
    }

    [Authorize]
    [HttpPost("logout")]
    public async Task<ActionResult<bool>> Logout([FromBody] LogoutUser.Command command)
    {
        return HandleResult(await Mediator.Send(command));
    }

    [Authorize]
    [HttpPut("{id}/change-password")]
    public async Task<ActionResult<bool>> ChangePassword(Guid id, [FromBody] ChangePasswordDto dto)
    {
        return HandleResult(await Mediator.Send(new ChangePassword.Command(id, dto)));
    }

    [Authorize]
    [HttpPut("{id}")]
    public async Task<ActionResult<UserResponseDto>> Update(Guid id, [FromBody] UpdateUserDto dto)
    {
        return HandleResult(await Mediator.Send(new UpdateUser.Command(id, dto)));
    }

    [Authorize(Roles = "Admin")]
    [HttpDelete("{id}")]
    public async Task<ActionResult<bool>> Delete(Guid id)
    {
        return HandleResult(await Mediator.Send(new DeleteUser.Command(id)));
    }

    [Authorize(Roles = "Admin")]
    [HttpGet]
    public async Task<ActionResult<IReadOnlyList<UserResponseDto>>> GetAll([FromQuery] GetAllUsers.Query query)
    {
        return HandleResult(await Mediator.Send(query));
    }

    [Authorize]
    [HttpGet("{id}")]
    public async Task<ActionResult<UserResponseDto>> GetById(Guid id)
    {
        return HandleResult(await Mediator.Send(new GetUserById.Query(id)));
    }
}