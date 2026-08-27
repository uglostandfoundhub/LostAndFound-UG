using LostAndFound.Shared.Dtos;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace LostAndFound.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public abstract class ApiControllerBase : ControllerBase
{
    protected string? CurrentUserId =>
        User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value;

    protected IActionResult ToAction(Result result) =>
        result.Succeeded ? Ok(new { success = true }) : StatusCode(result.StatusCode, new { error = result.Error });

    protected IActionResult ToAction<T>(Result<T> result) =>
        result.Succeeded ? Ok(result.Data) : StatusCode(result.StatusCode, new { error = result.Error });
}
