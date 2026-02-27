using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.RateLimiting;
using Microsoft.Extensions.Configuration;
using SolveIt.Api.Common.Extensions;
using SolveIt.Api.Contracts;
using SolveIt.Application.Auth.Commands.AuthenticateUser;
using SolveIt.Application.Auth.Commands.RefreshToken;
using SolveIt.Application.Auth.Commands.RegisterOrganizer;
using SolveIt.Application.Auth.Commands.RegisterUser;
using SolveIt.Application.Auth.Commands.RequestPasswordReset;
using SolveIt.Application.Auth.Commands.ResetPassword;
using SolveIt.Application.Common.DTOs.OrganizerAuthDTOs;
using SolveIt.Application.Common.DTOs.PasswordResetDTOs;
using SolveIt.Application.Organizers.Commands.RevokeAllSessions;
using SolveIt.Application.Organizers.Commands.RevokeOrganizerSession;
using System.Net;

namespace SolveIt.Api.Controllers;

/// <summary>
/// Unified authentication surface for all roles:
/// Organizer, Participant, and SuperAdmin.
/// Handles registration, login, refresh, logout,
/// revoke-all, profile retrieval, and password reset.
/// </summary>
[ApiController]
[Route("api/auth")]
public sealed class AuthController : ControllerBase
{
    private readonly IMediator _mediator;
    private readonly IConfiguration _configuration;

    public AuthController(
        IMediator mediator,
        IConfiguration configuration)
    {
        _mediator = mediator;
        _configuration = configuration;
    }

    [EnableRateLimiting("AuthModeratePolicy")]
    [HttpPost("participant/register")]
    public async Task<ActionResult<ApiResponse<Guid>>> Register(
        [FromBody] RegisterRequestDto request,
        CancellationToken cancellationToken)
    {
        var command = new RegisterUserCommand(
            request.Name,
            request.Email,
            request.PhoneNumber,
            request.Password,
            request.ConfirmPassword);

        var userId = await _mediator.Send(command, cancellationToken);

        return StatusCode(
            (int)HttpStatusCode.Created,
            ApiResponse<Guid>.Success(
                userId,
                "User registered successfully.",
                HttpStatusCode.Created));
    }

    [EnableRateLimiting("AuthModeratePolicy")]
    [HttpPost("organizer/register")]
    public async Task<ActionResult<ApiResponse<Guid>>> RegisterOrganizer(
    [FromBody] RegisterOrganizerRequestDto request,
    CancellationToken cancellationToken)
    {
        var command = new RegisterOrganizerCommand(
            request.Name,
            request.Email,
            request.PhoneNumber,
            request.Password,
            request.ConfirmPassword);

        var userId = await _mediator.Send(command, cancellationToken);

        return StatusCode(
            (int)HttpStatusCode.Created,
            ApiResponse<Guid>.Success(
                userId,
                "Organizer registered successfully.",
                HttpStatusCode.Created));
    }

    [EnableRateLimiting("AuthLoginPolicy")]
    [HttpPost("login")]
    public async Task<ActionResult<ApiResponse<string?>>> Login(
        [FromBody] LoginRequestDto request,
        CancellationToken cancellationToken)
    {
        var command = new AuthenticateUserCommand(
            request.Identifier,
            request.Password);

        var result = await _mediator.Send<AuthResponseDto>(command, cancellationToken);

        var accessTokenExpiryMinutes =
            _configuration.GetValue<int>("Jwt:ExpiryMinutes");

        var refreshTokenExpiryDays =
            _configuration.GetValue<int?>("Jwt:RefreshExpiryDays") ?? 7;

        Response.Cookies.Append(
            "access_token",
            result.AccessToken,
            new CookieOptions
            {
                HttpOnly = true,
                Secure = true,
                SameSite = SameSiteMode.None,
                Path = "/",
                Expires = DateTime.UtcNow.AddMinutes(accessTokenExpiryMinutes)
            });

        Response.Cookies.Append(
            "refresh_token",
            result.RefreshToken,
            new CookieOptions
            {
                HttpOnly = true,
                Secure = true,
                SameSite = SameSiteMode.None,
                Path = "/",
                Expires = DateTime.UtcNow.AddDays(refreshTokenExpiryDays)
            });

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

        var result = await _mediator.Send<AuthResponseDto>(
            new RefreshTokenCommand(refreshToken),
            cancellationToken);

        var accessTokenExpiryMinutes =
            _configuration.GetValue<int>("Jwt:ExpiryMinutes");

        var refreshTokenExpiryDays =
            _configuration.GetValue<int?>("Jwt:RefreshExpiryDays") ?? 7;

        Response.Cookies.Append(
            "access_token",
            result.AccessToken,
            new CookieOptions
            {
                HttpOnly = true,
                Secure = true,
                SameSite = SameSiteMode.None,
                Path = "/",
                Expires = DateTime.UtcNow.AddMinutes(accessTokenExpiryMinutes)
            });

        Response.Cookies.Append(
            "refresh_token",
            result.RefreshToken,
            new CookieOptions
            {
                HttpOnly = true,
                Secure = true,
                SameSite = SameSiteMode.None,
                Path = "/",
                Expires = DateTime.UtcNow.AddDays(refreshTokenExpiryDays)
            });

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

    [EnableRateLimiting("AuthModeratePolicy")]
    [HttpPost("logout")]
    public async Task<ActionResult<ApiResponse<string?>>> Logout(
        CancellationToken cancellationToken)
    {
        var refreshToken =
            Request.Cookies["refresh_token"];

        if (!string.IsNullOrWhiteSpace(refreshToken))
        {
            await _mediator.Send(
                new RevokeOrganizerSessionCommand(refreshToken),
                cancellationToken);
        }

        Response.Cookies.Delete("access_token");
        Response.Cookies.Delete("refresh_token");

        return Ok(ApiResponse<string?>.Success(
            null,
            "Logged out successfully."));
    }

    [EnableRateLimiting("AuthPasswordResetPolicy")]
    [HttpPost("request-password-reset")]
    public async Task<ActionResult<ApiResponse<string?>>> RequestPasswordReset(
        [FromBody] RequestPasswordResetDto request,
        CancellationToken cancellationToken)
    {
        await _mediator.Send(
            new RequestPasswordResetCommand(request.Email),
            cancellationToken);

        return Ok(ApiResponse<string?>.Success(
            null,
            "If the email exists, a password reset link has been sent."));
    }

    [EnableRateLimiting("AuthPasswordResetPolicy")]
    [HttpPost("reset-password")]
    public async Task<ActionResult<ApiResponse<string?>>> ResetPassword(
        [FromBody] ResetPasswordDto request,
        CancellationToken cancellationToken)
    {
        await _mediator.Send(
            new ResetPasswordCommand(
                request.Token,
                request.NewPassword,
                request.ConfirmPassword),
            cancellationToken);

        return Ok(ApiResponse<string?>.Success(
            null,
            "Password reset successfully."));
    }
}
