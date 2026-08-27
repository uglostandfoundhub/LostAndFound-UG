using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using LostAndFound.Domain.Entities;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;

namespace LostAndFound.Api.Services;

public interface IJwtService
{
    string GenerateToken(ApplicationUser user, IList<string> roles);
}

public class JwtService : IJwtService
{
    private readonly IConfiguration _configuration;

    public JwtService(IConfiguration configuration) => _configuration = configuration;

    public string GenerateToken(ApplicationUser user, IList<string> roles)
    {
        var issuer = _configuration["Jwt:Issuer"] ?? "LostAndFoundApi";
        var audience = _configuration["Jwt:Audience"] ?? "LostAndFoundClient";
        var key = _configuration["Jwt:Key"]
            ?? throw new InvalidOperationException("JWT signing key (Jwt:Key) is not configured.");
        var expiryMinutes = int.Parse(_configuration["Jwt:ExpiryMinutes"] ?? "60");

        var claims = new List<Claim>
        {
            new(ClaimTypes.NameIdentifier, user.Id),
            new(ClaimTypes.Email, user.Email ?? string.Empty),
            new(ClaimTypes.Name, user.FullName)
        };
        claims.AddRange(roles.Select(role => new Claim(ClaimTypes.Role, role)));

        var credentials = new SigningCredentials(
            new SymmetricSecurityKey(Encoding.UTF8.GetBytes(key)),
            SecurityAlgorithms.HmacSha256);

        var token = new JwtSecurityToken(
            issuer,
            audience,
            claims,
            expires: DateTime.UtcNow.AddMinutes(expiryMinutes),
            signingCredentials: credentials);

        return new JwtSecurityTokenHandler().WriteToken(token);
    }
}
