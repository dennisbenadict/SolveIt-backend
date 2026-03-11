using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.RateLimiting;
using SolveIt.Api.Common.Extensions;
using SolveIt.Api.Contracts;
using SolveIt.Application.Tournaments.Commands.CancelTournament;
using SolveIt.Application.Tournaments.Commands.CreateTournament;
using SolveIt.Application.Tournaments.Commands.JoinTournament;
using SolveIt.Application.Tournaments.Commands.PublishTournament;
using SolveIt.Application.Tournaments.Queries.GetLeaderboard;
using SolveIt.Application.Tournaments.Queries.GetMyTournaments;
using SolveIt.Application.Tournaments.Queries.GetTournamentById;
using System.Net;

namespace SolveIt.Api.Controllers;

[ApiController]
[Route("api/tournaments")]

public sealed class TournamentController : ControllerBase
{
    private readonly IMediator _mediator;

    public TournamentController(IMediator mediator)
    {
        _mediator = mediator;
    }

    [Authorize(Roles = "Organizer")]
    [EnableRateLimiting("AuthModeratePolicy")]
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

    [Authorize(Roles = "Organizer")]
    [EnableRateLimiting("AuthModeratePolicy")]
    [HttpPost("{id}/publish")]
    public async Task<ActionResult<ApiResponse<string>>> Publish(
    Guid id,
    CancellationToken cancellationToken)
    {
        var organizerId = User.GetUserId();

        await _mediator.Send(
            new PublishTournamentCommand(id, organizerId),
            cancellationToken);

        return Ok(ApiResponse<string>.Success(
            "Tournament published successfully.",
            "Operation successful",
            HttpStatusCode.OK));
    }

    [Authorize(Roles = "Organizer")]
    [EnableRateLimiting("AuthReadPolicy")]
    [HttpGet("my")]
    public async Task<ActionResult<ApiResponse<List<object>>>> GetMyTournaments(
    CancellationToken cancellationToken)
    {
        var organizerId = User.GetUserId();

        var tournaments = await _mediator.Send(
            new GetMyTournamentsQuery(organizerId),
            cancellationToken);

        var result = tournaments.Select(t => new
        {
            t.Id,
            t.Title,
            t.Description,
            t.StartTimeUtc,
            t.EndTimeUtc,
            t.Status,
            t.IsFreeTrial,
            t.CreatedAtUtc
        });

        return Ok(ApiResponse<object>.Success(
            result,
            "Tournaments retrieved"));
    }

    [Authorize]
    [EnableRateLimiting("AuthReadPolicy")]
    [HttpGet("{id}")]
    public async Task<ActionResult<ApiResponse<object>>> GetTournament(
    Guid id,
    CancellationToken cancellationToken)
    {
        var tournament = await _mediator.Send(
            new GetTournamentByIdQuery(id),
            cancellationToken);

        var result = new
        {
            tournament.Id,
            tournament.Title,
            tournament.Description,
            tournament.StartTimeUtc,
            tournament.EndTimeUtc,
            tournament.Status,
            tournament.IsFreeTrial,
            tournament.CreatedAtUtc
        };

        return Ok(ApiResponse<object>.Success(
            result,
            "Tournament retrieved"));
    }

    [Authorize(Roles = "Organizer")]
    [EnableRateLimiting("AuthModeratePolicy")]
    [HttpPost("{id}/cancel")]
    public async Task<ActionResult<ApiResponse<string>>> Cancel(
    Guid id,
    CancellationToken cancellationToken)
    {
        var organizerId = User.GetUserId();

        await _mediator.Send(
            new CancelTournamentCommand(id, organizerId),
            cancellationToken);

        return Ok(ApiResponse<string>.Success(
            "Tournament cancelled successfully.",
            "Operation successful",
            HttpStatusCode.OK));
    }

    [Authorize(Roles = "Participant")]
    [EnableRateLimiting("AuthModeratePolicy")]
    [HttpPost("{id}/join")]
    public async Task<ActionResult<ApiResponse<string>>> JoinTournament(
    Guid id,
    CancellationToken cancellationToken)
    {
        var participantId = User.GetUserId();

        await _mediator.Send(
            new JoinTournamentCommand(id, participantId),
            cancellationToken);

        return Ok(ApiResponse<string>.Success(
            "Successfully joined tournament",
            "Operation successful",
            HttpStatusCode.OK));
    }

    [Authorize]
    [EnableRateLimiting("AuthReadPolicy")]
    [HttpGet("{id}/leaderboard")]
    public async Task<ActionResult<ApiResponse<object>>> GetLeaderboard(
    Guid id,
    CancellationToken cancellationToken)
    {
        var result = await _mediator.Send(
            new GetLeaderboardQuery(id),
            cancellationToken);

        return Ok(ApiResponse<object>.Success(
            result,
            "Leaderboard retrieved"));
    }
}