using LostAndFound.Api.Services;
using LostAndFound.Shared.Dtos.Auth;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace LostAndFound.Api.Controllers;

[Route("api/auth")]
public class AuthController : ApiControllerBase
{
    private readonly IAuthService _auth;

    public AuthController(IAuthService auth) => _auth = auth;

    [HttpPost("register")]
    [AllowAnonymous]
    public async Task<IActionResult> Register(RegisterRequest request)
    {
        var result = await _auth.RegisterAsync(request);
        return result is null
            ? BadRequest(new { error = "Registration failed. Email may already be in use or password too weak." })
            : Ok(result);
    }

    [HttpPost("login")]
    [AllowAnonymous]
    public async Task<IActionResult> Login(LoginRequest request)
    {
        var result = await _auth.LoginAsync(request);
        return result is null
            ? Unauthorized(new { error = "Invalid credentials." })
            : Ok(result);
    }

    [HttpGet("me")]
    public async Task<IActionResult> Me()
    {
        var user = await _auth.GetCurrentUserAsync(CurrentUserId!);
        return user is null ? NotFound(new { error = "User not found." }) : Ok(user);
    }
}
