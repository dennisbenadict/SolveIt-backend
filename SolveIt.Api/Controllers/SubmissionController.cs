using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.RateLimiting;
using SolveIt.Api.Common.Extensions;
using SolveIt.Api.Contracts;
using SolveIt.Application.Interfaces;
using SolveIt.Application.Submissions.Commands.SubmitSolution;
using SolveIt.Application.Submissions.Queries.GetMySubmissions;
using SolveIt.Application.Submissions.Queries.GetSubmissionById;
using System.Net;

namespace SolveIt.Api.Controllers;

[ApiController]
[Route("api/submissions")]
public sealed class SubmissionController : ControllerBase
{
    private readonly IMediator _mediator;
    private readonly ISubmissionRepository _submissionRepository;

    public SubmissionController(
        IMediator mediator,
        ISubmissionRepository submissionRepository)
    {
        _mediator = mediator;
        _submissionRepository = submissionRepository;
    }

    [Authorize(Roles = "Participant")]
    [EnableRateLimiting("AuthModeratePolicy")]
    [HttpPost]
    public async Task<ActionResult<ApiResponse<Guid>>> SubmitSolution(
        [FromBody] SubmitSolutionRequestDto request,
        CancellationToken cancellationToken)
    {
        var participantId = User.GetUserId();

        var command = new SubmitSolutionCommand(
            request.TournamentId,
            request.ProblemId,
            participantId,
            request.Language,
            request.SourceCode);

        var submissionId =
            await _mediator.Send(command, cancellationToken);

        return Ok(ApiResponse<Guid>.Success(
            submissionId,
            "Submission received.",
            HttpStatusCode.OK));
    }

    [Authorize(Roles = "Participant")]
    [EnableRateLimiting("AuthReadPolicy")]
    [HttpGet("my")]
    public async Task<ActionResult<ApiResponse<object>>> GetMySubmissions(
        CancellationToken cancellationToken)
    {
        var participantId = User.GetUserId();

        var result = await _mediator.Send(
            new GetMySubmissionsQuery(participantId),
            cancellationToken);

        return Ok(ApiResponse<object>.Success(
            result,
            "Submissions retrieved"));
    }

    [Authorize]
    [EnableRateLimiting("AuthReadPolicy")]
    [HttpGet("{id}")]
    public async Task<ActionResult<ApiResponse<object>>> GetSubmission(
    Guid id,
    CancellationToken cancellationToken)
    {
        var result = await _mediator.Send(
            new GetSubmissionByIdQuery(id),
            cancellationToken);

        return Ok(ApiResponse<object>.Success(
            result,
            "Submission status retrieved"));
    }
}
