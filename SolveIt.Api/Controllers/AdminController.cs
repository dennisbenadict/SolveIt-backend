using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SolveIt.Api.Contracts;
using System.Net;
using SolveIt.Application.Admin.UnblockUser;
using SolveIt.Application.Admin.Commands.BlockUser;

namespace SolveIt.Api.Controllers;

[ApiController]
[Route("api/admin")]
[Authorize(Roles = "SuperAdmin")]
public sealed class AdminController : ControllerBase
{
    private readonly IMediator _mediator;

    public AdminController(IMediator mediator)
    {
        _mediator = mediator;
    }

    [HttpGet("health")]
    public ActionResult<ApiResponse<string>> Health()
    {
        return Ok(ApiResponse<string>.Success(
            "Admin access confirmed",
            "Admin endpoint reachable.",
            HttpStatusCode.OK));
    }

    [HttpPatch("users/{userId:guid}/block")]
    public async Task<ActionResult<ApiResponse<string>>> BlockUser(
    Guid userId,
    CancellationToken cancellationToken)
    {
        await _mediator.Send(
            new BlockUserCommand(userId),
            cancellationToken);

        return Ok(
            ApiResponse<string>.Success(
                "Operation successful",
                "User blocked successfully.",
                HttpStatusCode.OK));
    }

    [HttpPatch("users/{userId:guid}/unblock")]
    public async Task<ActionResult<ApiResponse<string>>> UnblockUser(
        Guid userId,
        CancellationToken cancellationToken)
    {
        await _mediator.Send(
            new UnblockUserCommand(userId),
            cancellationToken);

        return Ok(
            ApiResponse<string>.Success(
                "Operation successful",
                "User unblocked successfully.",
                HttpStatusCode.OK));
    }
}
