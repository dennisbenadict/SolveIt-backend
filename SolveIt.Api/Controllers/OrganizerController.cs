using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity.Data;
using Microsoft.AspNetCore.Mvc;
using SolveIt.Application.Common.DTOs.OrganizerAuthDTOs;
using SolveIt.Application.Organizers.Commands.RevokeOrganizerSession;
using Solvelt.Api.Contracts;
using Solvelt.Application.Organizers.Commands.LoginOrganizer;
using Solvelt.Application.Organizers.Commands.RefreshOrganizerToken;
using Solvelt.Application.Organizers.Commands.RegisterOrganizer;
using System.Net;

namespace Solvelt.Api.Controllers;

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

        return Ok(ApiResponse<AuthResponseDto>.Success(
            result,
            "Login successful."));
    }

    [HttpPost("refresh")]
    public async Task<ActionResult<ApiResponse<AuthResponseDto>>> Refresh(
        [FromBody] RefreshTokenRequestDto request,
        CancellationToken cancellationToken)
    {
        var result =
            await _mediator.Send(
                new RefreshOrganizerTokenCommand(request.RefreshToken),
                cancellationToken);

        return Ok(ApiResponse<AuthResponseDto>.Success(
            result,
            "Token refreshed successfully."));
    }

    [HttpPost("logout")]
    public async Task<ActionResult<ApiResponse<string>>> Logout(
        [FromBody] RefreshRequest request,
        CancellationToken cancellationToken)
    {
        await _mediator.Send(
            new RevokeOrganizerSessionCommand(request.RefreshToken),
            cancellationToken);

        return Ok(ApiResponse<string>.Success(
            null,
            "Logged out successfully."));
    }

    [Authorize]
    [HttpGet("me")]
    public ActionResult<ApiResponse<OrganizerProfileDto>> Me()
    {
        var userId = User.FindFirst("sub")?.Value;
        var email = User.FindFirst("email")?.Value;
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
}


