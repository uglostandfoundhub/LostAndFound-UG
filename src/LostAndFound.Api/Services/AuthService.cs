using LostAndFound.Domain.Constants;
using LostAndFound.Domain.Entities;
using LostAndFound.Shared.Dtos;
using LostAndFound.Shared.Dtos.Auth;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Configuration;

namespace LostAndFound.Api.Services;

public interface IAuthService
{
    Task<AuthResponse?> RegisterAsync(RegisterRequest request);
    Task<AuthResponse?> LoginAsync(LoginRequest request);
    Task<UserDto?> GetCurrentUserAsync(string userId);
}

public class AuthService : IAuthService
{
    private readonly UserManager<ApplicationUser> _users;
    private readonly RoleManager<IdentityRole> _roles;
    private readonly IJwtService _jwt;
    private readonly IConfiguration _configuration;

    public AuthService(
        UserManager<ApplicationUser> users,
        RoleManager<IdentityRole> roles,
        IJwtService jwt,
        IConfiguration configuration)
    {
        _users = users;
        _roles = roles;
        _jwt = jwt;
        _configuration = configuration;
    }

    public async Task<AuthResponse?> RegisterAsync(RegisterRequest request)
    {
        var role = Roles.All.Contains(request.Role) ? request.Role : Roles.Student;

        var user = new ApplicationUser
        {
            UserName = request.Email,
            Email = request.Email,
            FullName = request.FullName,
            StudentId = request.StudentId,
            Department = request.Department
        };

        var result = await _users.CreateAsync(user, request.Password);
        if (!result.Succeeded) return null;

        if (!await _roles.RoleExistsAsync(role))
            await _roles.CreateAsync(new IdentityRole(role));

        await _users.AddToRoleAsync(user, role);
        return await BuildResponse(user);
    }

    public async Task<AuthResponse?> LoginAsync(LoginRequest request)
    {
        var user = await _users.FindByEmailAsync(request.Email);
        if (user is null || !user.IsActive) return null;
        if (!await _users.CheckPasswordAsync(user, request.Password)) return null;
        return await BuildResponse(user);
    }

    public async Task<UserDto?> GetCurrentUserAsync(string userId)
    {
        var user = await _users.FindByIdAsync(userId);
        return user is null ? null : await ToDto(user);
    }

    private async Task<AuthResponse> BuildResponse(ApplicationUser user)
    {
        var roles = await _users.GetRolesAsync(user);
        var expiryMinutes = int.Parse(_configuration["Jwt:ExpiryMinutes"] ?? "60");

        return new AuthResponse
        {
            Token = _jwt.GenerateToken(user, roles),
            ExpiresAt = DateTime.UtcNow.AddMinutes(expiryMinutes),
            User = await ToDto(user, roles)
        };
    }

    private async Task<UserDto> ToDto(ApplicationUser user, IList<string>? roles = null)
    {
        roles ??= await _users.GetRolesAsync(user);
        return new UserDto
        {
            Id = user.Id,
            Email = user.Email ?? string.Empty,
            FullName = user.FullName,
            StudentId = user.StudentId,
            Department = user.Department,
            IsActive = user.IsActive,
            Roles = roles
        };
    }
}
