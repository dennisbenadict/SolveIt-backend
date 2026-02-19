using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity.Data;
using Microsoft.AspNetCore.Mvc;
using SolveIt.Application.Common.DTOs.OrganizerAuthDTOs;
using SolveIt.Application.Organizers.Commands.RevokeOrganizerSession;
using Solvelt.Application.Organizers.Commands.LoginOrganizer;
using Solvelt.Application.Organizers.Commands.RefreshOrganizerToken;
using Solvelt.Application.Organizers.Commands.RegisterOrganizer;

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
    public async Task<ActionResult<Guid>> Register(
        [FromBody] RegisterOrganizerRequestDto request,
        CancellationToken cancellationToken)
    {
        var command = new RegisterOrganizerCommand(
            request.Email,
            request.Name,
            request.PhoneNumber,
            request.Password,
            request.ConfirmPassword);
        var organizerId = await _mediator.Send(command, cancellationToken);
        return CreatedAtAction(nameof(Register), new { id = organizerId }, null);
    }

    [HttpPost("login")]
    public async Task<ActionResult<AuthResponseDto>> Login(
    [FromBody] LoginOrganizerRequestDto request,
    CancellationToken cancellationToken)
    {
        var command = new LoginOrganizerCommand(
            request.Identifier,
            request.Password);
        var result = await _mediator.Send(command, cancellationToken);
        return Ok(result);
    }

    [HttpPost("refresh")]
    public async Task<ActionResult<AuthResponseDto>> Refresh(
        [FromBody] RefreshTokenRequestDto request)
    {
        var result = await _mediator.Send(
            new RefreshOrganizerTokenCommand(request.RefreshToken));

        return Ok(result);
    }

    [HttpPost("logout")]
    public async Task<ActionResult> Logout(
    [FromBody] RefreshRequest request, CancellationToken cancellationToken)
    {
        await _mediator.Send(
            new RevokeOrganizerSessionCommand(request.RefreshToken),cancellationToken);

        return NoContent();
    }

    [Authorize]
    [HttpGet("me")]
    public ActionResult<OrganizerProfileDto> Me()
    {
        var userId = User.FindFirst("sub")?.Value;
        var email = User.FindFirst("email")?.Value;
        var name = User.FindFirst("name")?.Value;

        if (userId is null || email is null || name is null)
            return Unauthorized();

        return Ok(new OrganizerProfileDto(email, name));
    }
}

