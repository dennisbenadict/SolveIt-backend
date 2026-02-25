using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SolveIt.Api.Contracts;
using System.Net;

namespace SolveIt.Api.Controllers;

[ApiController]
[Route("api/admin")]
[Authorize(Roles = "SuperAdmin")]
public sealed class AdminController : ControllerBase
{
    [HttpGet("health")]
    public ActionResult<ApiResponse<string>> Health()
    {
        return Ok(ApiResponse<string>.Success(
            "Admin access confirmed",
            "Admin endpoint reachable.",
            HttpStatusCode.OK));
    }
}
