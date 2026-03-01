using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.RateLimiting;
using SolveIt.Api.Common.Extensions;
using SolveIt.Api.Contracts;
using SolveIt.Application.Tournaments.Commands.CreateTournament;
using System.Net;

namespace SolveIt.Api.Controllers;

[ApiController]
[Route("api/tournaments")]
[Authorize(Roles = "Organizer")]
[EnableRateLimiting("AuthModeratePolicy")]
public sealed class TournamentController : ControllerBase
{
    private readonly IMediator _mediator;

    public TournamentController(IMediator mediator)
    {
        _mediator = mediator;
    }

    [HttpPost]
    public async Task<ActionResult<ApiResponse<Guid>>> CreateTournament(
        [FromBody] CreateTournamentRequestDto request,
        CancellationToken cancellationToken)
    {
        var organizerId = User.GetUserId(); // your existing extension

        var ipAddress = HttpContext.Connection.RemoteIpAddress?.ToString();

        var command = new CreateTournamentCommand(
            organizerId,
            request.Title,
            request.Description,
            request.DeviceFingerprint,
            ipAddress,
            request.StartTimeUtc,
            request.EndTimeUtc);

        var tournamentId = await _mediator.Send(command, cancellationToken);

        return Ok(
            ApiResponse<Guid>.Success(
                tournamentId,
                "Tournament created successfully.",
                HttpStatusCode.OK));
    }
}