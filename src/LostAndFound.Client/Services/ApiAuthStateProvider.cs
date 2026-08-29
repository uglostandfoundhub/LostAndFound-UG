using System.Security.Claims;
using LostAndFound.Client.Services;
using LostAndFound.Shared.Dtos;
using Microsoft.AspNetCore.Components.Authorization;

namespace LostAndFound.Client.Services;

public class ApiAuthStateProvider : AuthenticationStateProvider
{
    private readonly ITokenStorage _tokenStorage;
    private readonly ApiClient _api;
    private readonly ClaimsPrincipal _anonymous = new(new ClaimsIdentity());
    private ClaimsPrincipal? _current;

    public ApiAuthStateProvider(ITokenStorage tokenStorage, ApiClient api)
    {
        _tokenStorage = tokenStorage;
        _api = api;
    }

    public override async Task<AuthenticationState> GetAuthenticationStateAsync()
    {
        if (_current is not null)
            return new AuthenticationState(_current);

        var token = await _tokenStorage.GetTokenAsync();
        if (string.IsNullOrEmpty(token))
            return new AuthenticationState(_anonymous);

        var user = await _api.GetCurrentUserAsync();
        if (user is null)
        {
            await _tokenStorage.RemoveTokenAsync();
            return new AuthenticationState(_anonymous);
        }

        _current = BuildPrincipal(user);
        return new AuthenticationState(_current);
    }

    public async Task LoginAsync(string token)
    {
        await _tokenStorage.SetTokenAsync(token);
        var user = await _api.GetCurrentUserAsync();
        _current = user is null ? _anonymous : BuildPrincipal(user);
        NotifyAuthenticationStateChanged(GetAuthenticationStateAsync());
    }

    public async Task LogoutAsync()
    {
        await _tokenStorage.RemoveTokenAsync();
        _current = null;
        NotifyAuthenticationStateChanged(Task.FromResult(new AuthenticationState(_anonymous)));
    }

    private static ClaimsPrincipal BuildPrincipal(UserDto user)
    {
        var claims = new List<Claim>
        {
            new(ClaimTypes.NameIdentifier, user.Id),
            new(ClaimTypes.Name, user.FullName),
            new(ClaimTypes.Email, user.Email)
        };
        claims.AddRange(user.Roles.Select(role => new Claim(ClaimTypes.Role, role)));
        return new ClaimsPrincipal(new ClaimsIdentity(claims, "jwt"));
    }
}
