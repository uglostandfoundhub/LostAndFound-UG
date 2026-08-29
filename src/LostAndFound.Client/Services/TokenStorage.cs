using Microsoft.JSInterop;

namespace LostAndFound.Client.Services;

public interface ITokenStorage
{
    Task<string?> GetTokenAsync();
    Task SetTokenAsync(string token);
    Task RemoveTokenAsync();
}

public class TokenStorage : ITokenStorage
{
    private const string Key = "laf_token";
    private readonly IJSRuntime _js;

    public TokenStorage(IJSRuntime js) => _js = js;

    public Task<string?> GetTokenAsync() =>
        _js.InvokeAsync<string?>("localStorage.getItem", Key).AsTask();

    public Task SetTokenAsync(string token) =>
        _js.InvokeAsync<object>("localStorage.setItem", Key, token).AsTask();

    public Task RemoveTokenAsync() =>
        _js.InvokeAsync<object>("localStorage.removeItem", Key).AsTask();
}
