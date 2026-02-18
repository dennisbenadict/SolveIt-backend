using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity.Data;
using Microsoft.AspNetCore.Mvc;
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
        RegisterOrganizerCommand command,
        CancellationToken cancellationToken)
    {
        var organizerId = await _mediator.Send(command, cancellationToken);
        return CreatedAtAction(nameof(Register), new { id = organizerId }, null);
    }

    [HttpPost("login")]
    public async Task<ActionResult<LoginResponse>> Login(
    [FromBody] LoginOrganizerCommand command,
    CancellationToken cancellationToken)
    {
        var result = await _mediator.Send(command, cancellationToken);
        return Ok(result);
    }

    [HttpPost("refresh")]
    public async Task<ActionResult<LoginResponse>> Refresh(
        [FromBody] RefreshRequest request)
    {
        var result = await _mediator.Send(
            new RefreshOrganizerTokenCommand(request.RefreshToken));

        return Ok(result);
    }

    [HttpPost("logout")]
    public async Task<IActionResult> Logout(
    [FromBody] RefreshRequest request)
    {
        await _mediator.Send(
            new RevokeOrganizerSessionCommand(request.RefreshToken));

        return NoContent();
    }

    [Authorize]
    [HttpGet("me")]
    public ActionResult<object> Me()
    {
        var userId = User.FindFirst("sub")?.Value;
        var email = User.FindFirst("email")?.Value;
        var name = User.FindFirst("name")?.Value;

        if (userId is null)
            return Unauthorized();

        return Ok(new
        {
            Id = userId,
            Email = email,
            Name = name
        });
    }
}

