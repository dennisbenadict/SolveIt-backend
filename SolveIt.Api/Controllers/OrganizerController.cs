using MediatR;
using Microsoft.AspNetCore.Mvc;
using Solvelt.Application.Organizers.Commands.LoginOrganizer;
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

}

