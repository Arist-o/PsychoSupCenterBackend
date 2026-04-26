
using Application.Users.DTOs;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using PsychoSupCenterBackend.Application.Users.Commands;
using PsychoSupCenterBackend.Application.Users.DTOs;
using PsychoSupCenterBackend.Application.Users.Queries;

namespace API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class UsersController(ISender sender) : ControllerBase
{
    // ── Auth endpoints (public) ───────────────────────────────────

    /// <summary>POST /api/users/register — Реєстрація нового користувача</summary>
    [HttpPost("register")]
    [ProducesResponseType(typeof(AuthResponseDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> Register(
        [FromBody] RegisterDto dto,
        CancellationToken cancellationToken)
    {
        var ipAddress = HttpContext.Connection.RemoteIpAddress?.ToString();
        var result = await sender.Send(
            new RegisterUser.Command(dto, ipAddress),
            cancellationToken);

        return result.IsSuccess
            ? Ok(result.Value)
            : BadRequest(new { error = result.Error });
    }

    /// <summary>POST /api/users/login — Вхід у систему</summary>
    [HttpPost("login")]
    [ProducesResponseType(typeof(AuthResponseDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> Login(
        [FromBody] LoginDto dto,
        CancellationToken cancellationToken)
    {
        var ipAddress = HttpContext.Connection.RemoteIpAddress?.ToString();
        var result = await sender.Send(
            new LoginUser.Command(dto, ipAddress),
            cancellationToken);

        return result.IsSuccess
            ? Ok(result.Value)
            : BadRequest(new { error = result.Error });
    }

    /// <summary>POST /api/users/refresh — Оновлення access token через refresh token</summary>
    [HttpPost("refresh")]
    [ProducesResponseType(typeof(AuthResponseDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> Refresh(
        [FromBody] RefreshTokenDto dto,
        CancellationToken cancellationToken)
    {
        var ipAddress = HttpContext.Connection.RemoteIpAddress?.ToString();
        var result = await sender.Send(
            new RefreshTokenCommand.Command(dto.RefreshToken, ipAddress),
            cancellationToken);

        return result.IsSuccess
            ? Ok(result.Value)
            : BadRequest(new { error = result.Error });
    }

    /// <summary>POST /api/users/logout — Вихід із системи (відкликання refresh token)</summary>
    [HttpPost("logout")]
    [Authorize]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> Logout(
        [FromBody] LogoutDto dto,
        CancellationToken cancellationToken)
    {
        var result = await sender.Send(
            new LogoutUser.Command(dto.RefreshToken),
            cancellationToken);

        return result.IsSuccess
            ? Ok(new { message = "Виконано вихід із системи." })
            : BadRequest(new { error = result.Error });
    }

    // ── Account management endpoints (authorized) ─────────────────

    /// <summary>PUT /api/users/{userId}/password — Зміна пароля</summary>
    [HttpPut("{userId:guid}/password")]
    [Authorize]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> ChangePassword(
        [FromRoute] Guid userId,
        [FromBody] ChangePasswordDto dto,
        CancellationToken cancellationToken)
    {
        var result = await sender.Send(
            new ChangePassword.Command(userId, dto),
            cancellationToken);

        return result.IsSuccess
            ? Ok(new { message = "Пароль успішно змінено." })
            : BadRequest(new { error = result.Error });
    }

    /// <summary>DELETE /api/users/{userId} — Деактивація акаунту (soft delete)</summary>
    [HttpDelete("{userId:guid}")]
    [Authorize]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> Delete(
        [FromRoute] Guid userId,
        CancellationToken cancellationToken)
    {
        var result = await sender.Send(
            new DeleteUser.Command(userId),
            cancellationToken);

        return result.IsSuccess
            ? Ok(new { message = "Акаунт деактивовано." })
            : BadRequest(new { error = result.Error });
    }

 
    // ── Queries (Data Retrieval) ──────────────────────────────────

    /// <summary>GET /api/users — Отримання списку всіх користувачів (з пагінацією)</summary>
    [HttpGet]
    [Authorize] // Обов'язково з токеном, як ми вказували в Postman
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> GetAll(
        [FromQuery] int page = 1,
        [FromQuery] int pageSize = 10,
        CancellationToken cancellationToken = default)
    {
        // Переконайся, що клас запиту називається саме GetAllUsers.Query. 
        // Якщо інакше - просто підправ назву.
        var result = await sender.Send(
            new GetAllUsers.Query(page, pageSize),
            cancellationToken);

        return result.IsSuccess
            ? Ok(result.Value)
            : BadRequest(new { error = result.Error });
    }

    /// <summary>GET /api/users/{userId} — Отримання користувача за ID</summary>
    [HttpGet("{userId:guid}")]
    [Authorize]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> GetById(
        [FromRoute] Guid userId,
        CancellationToken cancellationToken)
    {
        var result = await sender.Send(
            new GetUserById.Query(userId),
            cancellationToken);

        return result.IsSuccess
            ? Ok(result.Value)
            : BadRequest(new { error = result.Error });
    }
}



