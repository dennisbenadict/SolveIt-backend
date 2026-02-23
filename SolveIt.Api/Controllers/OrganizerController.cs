using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity.Data;
using Microsoft.AspNetCore.Mvc;
using SolveIt.Api.Contracts;
using SolveIt.Application.Common.DTOs.OrganizerAuthDTOs;
using SolveIt.Application.Common.DTOs.PasswordResetDTOs;
using SolveIt.Application.Organizers.Commands.LoginOrganizer;
using SolveIt.Application.Organizers.Commands.RefreshOrganizerToken;
using SolveIt.Application.Organizers.Commands.RegisterOrganizer;
using SolveIt.Application.Organizers.Commands.RequestPasswordReset;
using SolveIt.Application.Organizers.Commands.RevokeAllSessions;
using SolveIt.Application.Organizers.Commands.RevokeOrganizerSession;
using System.Net;
using System.Security.Claims;
using SolveIt.Api.Common.Extensions;

namespace SolveIt.Api.Controllers;

[ApiController]
[Route("api/organizers")]
public sealed class OrganizerController : ControllerBase
{
    private readonly IMediator _mediator;

    public OrganizerController(IMediator mediator)
    {
        _mediator = mediator;
    }

    [HttpPost]
    public async Task<ActionResult<ApiResponse<Guid>>> Register(
        [FromBody] RegisterOrganizerRequestDto request,
        CancellationToken cancellationToken)
    {
        var command = new RegisterOrganizerCommand(
            request.Email,
            request.Name,
            request.PhoneNumber,
            request.Password,
            request.ConfirmPassword);

        var organizerId =
            await _mediator.Send(command, cancellationToken);

        var response = ApiResponse<Guid>.Success(
            organizerId,
            "Organizer registered successfully.",
            HttpStatusCode.Created);

        return StatusCode((int)HttpStatusCode.Created, response);
    }

    [HttpPost("login")]
    public async Task<ActionResult<ApiResponse<AuthResponseDto>>> Login(
        [FromBody] LoginOrganizerRequestDto request,
        CancellationToken cancellationToken)
    {
        var command = new LoginOrganizerCommand(
            request.Identifier,
            request.Password);

        var result =
            await _mediator.Send(command, cancellationToken);

        Response.Cookies.Append(
            "access_token",
            result.AccessToken,
            new CookieOptions
            {
                HttpOnly = true,
                Secure = true,
                SameSite = SameSiteMode.None,
                Path = "/",
                Expires = DateTime.UtcNow.AddMinutes(15)
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
                Expires = DateTime.UtcNow.AddDays(7)
            });

        return Ok(ApiResponse<string>.Success(
            null,
            "Login successful"));
    }

    [HttpPost("refresh")]
    public async Task<ActionResult<ApiResponse<string>>> Refresh(
        CancellationToken cancellationToken)
    {
        var refreshToken =
            Request.Cookies["refresh_token"];

        if (string.IsNullOrWhiteSpace(refreshToken))
            return Unauthorized();

        var result =
            await _mediator.Send(
                new RefreshOrganizerTokenCommand(refreshToken),
                cancellationToken);

        // Overwritten cookies with rotated tokens
        Response.Cookies.Append(
            "access_token",
            result.AccessToken,
            new CookieOptions
            {
                HttpOnly = true,
                Secure = true,
                SameSite = SameSiteMode.None,
                Path = "/",
                Expires = DateTime.UtcNow.AddMinutes(15)
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
                Expires = DateTime.UtcNow.AddDays(7)
            });

        return Ok(ApiResponse<string>.Success(
            null,
            "Token refreshed successfully."));
    }

    [HttpPost("logout")]
    public async Task<ActionResult<ApiResponse<string>>> Logout(
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

        // Clear cookies
        Response.Cookies.Delete("access_token");
        Response.Cookies.Delete("refresh_token");

        return Ok(ApiResponse<string>.Success(
            null,
            "Logged out successfully."));
    }

    [Authorize]
    [HttpGet("me")]
    public ActionResult<ApiResponse<OrganizerProfileDto>> Me()
    {
        var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        var email = User.FindFirst(ClaimTypes.Email)?.Value;
        var name = User.FindFirst("name")?.Value;

        if (userId is null || email is null || name is null)
            return Unauthorized(ApiResponse<string>.Fail(
                new List<string> { "Unauthorized" },
                "Authentication failed",
                HttpStatusCode.Unauthorized));

        return Ok(ApiResponse<OrganizerProfileDto>.Success(
            new OrganizerProfileDto(email, name),
            "Profile retrieved successfully."));
    }

    [HttpPost("request-password-reset")]
    public async Task<ActionResult<ApiResponse<string>>> RequestPasswordReset(
        [FromBody] RequestPasswordResetDto request,
        CancellationToken cancellationToken)
    {
        await _mediator.Send(
            new RequestPasswordResetCommand(request.Email),
            cancellationToken);

        return Ok(ApiResponse<string>.Success(
            null,
            "If the email exists, a password reset link has been sent."));
    }

    [HttpPost("reset-password")]
    public async Task<ActionResult<ApiResponse<string>>> ResetPassword(
        [FromBody] ResetPasswordDto request,
        CancellationToken cancellationToken)
    {
        await _mediator.Send(
            new ResetPasswordCommand(
                request.Token,
                request.NewPassword,
                request.ConfirmPassword),
            cancellationToken);

        return Ok(ApiResponse<string>.Success(
            null,
            "Password reset successfully."));
    }

    [Authorize]
    [HttpPost("revoke-all")]
    public async Task<ActionResult<ApiResponse<string>>> RevokeAll(
    CancellationToken cancellationToken)
    {
        var organizerId = User.GetUserId();

        await _mediator.Send(
            new RevokeAllSessionsCommand(organizerId),
            cancellationToken);

        return Ok(ApiResponse<string>.Success(
            null,
            "All sessions revoked successfully."));
    }
}


