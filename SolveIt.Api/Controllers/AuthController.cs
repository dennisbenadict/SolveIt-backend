using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.RateLimiting;
using SolveIt.Api.Common.Extensions;
using SolveIt.Api.Contracts;
using SolveIt.Application.Auth.Commands;
using SolveIt.Application.Organizers.Commands.RevokeAllSessions;
using System.Net;

namespace SolveIt.Api.Controllers;

[ApiController]
[Route("api/auth")]
public sealed class AuthController : ControllerBase
{
    private readonly IMediator _mediator;

    public AuthController(IMediator mediator)
    {
        _mediator = mediator;
    }

    [EnableRateLimiting("AuthModeratePolicy")]
    [HttpPost("register")]
    public async Task<ActionResult<ApiResponse<Guid>>> Register(
        [FromBody] RegisterUserCommand command,
        CancellationToken cancellationToken)
    {
        var userId = await _mediator.Send(command, cancellationToken);

        return StatusCode(
            (int)HttpStatusCode.Created,
            ApiResponse<Guid>.Success(
                userId,
                "User registered successfully.",
                HttpStatusCode.Created));
    }

    [EnableRateLimiting("AuthLoginPolicy")]
    [HttpPost("login")]
    public async Task<ActionResult<ApiResponse<string?>>> Login(
        [FromBody] AuthenticateUserCommand command,
        CancellationToken cancellationToken)
    {
        var result = await _mediator.Send(command, cancellationToken);

        Response.Cookies.Append("access_token", result.AccessToken, result.AccessCookieOptions);
        Response.Cookies.Append("refresh_token", result.RefreshToken, result.RefreshCookieOptions);

        return Ok(ApiResponse<string?>.Success(
            null,
            "Login successful"));
    }

    [EnableRateLimiting("AuthRefreshPolicy")]
    [HttpPost("refresh")]
    public async Task<ActionResult<ApiResponse<string?>>> Refresh(
        CancellationToken cancellationToken)
    {
        var refreshToken = Request.Cookies["refresh_token"];

        if (string.IsNullOrWhiteSpace(refreshToken))
            return Unauthorized();

        var result = await _mediator.Send(
            new RefreshTokenCommand(refreshToken),
            cancellationToken);

        Response.Cookies.Append("access_token", result.AccessToken, result.AccessCookieOptions);
        Response.Cookies.Append("refresh_token", result.RefreshToken, result.RefreshCookieOptions);

        return Ok(ApiResponse<string?>.Success(
            null,
            "Token refreshed"));
    }

    [Authorize]
    [HttpPost("revoke-all")]
    public async Task<ActionResult<ApiResponse<string?>>> RevokeAll(
        CancellationToken cancellationToken)
    {
        var userId = User.GetUserId();

        await _mediator.Send(new RevokeAllSessionsCommand(userId), cancellationToken);

        return Ok(ApiResponse<string?>.Success(
            null,
            "All sessions revoked"));
    }

    [Authorize]
    [HttpGet("me")]
    public ActionResult<ApiResponse<object>> Me()
    {
        var userId = User.GetUserId();
        var email = User.GetEmail();
        var name = User.GetName();
        var role = User.GetRole();

        return Ok(ApiResponse<object>.Success(
            new { userId, email, name, role },
            "Profile retrieved"));
    }
}
