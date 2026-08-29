using System.Net.Http.Headers;
using System.Threading;
using System.Threading.Tasks;

namespace LostAndFound.Client.Services;

public class AuthMessageHandler : DelegatingHandler
{
    private readonly ITokenStorage _tokenStorage;

    public AuthMessageHandler(ITokenStorage tokenStorage) => _tokenStorage = tokenStorage;

    protected override async Task<HttpResponseMessage> SendAsync(
        HttpRequestMessage request,
        CancellationToken cancellationToken)
    {
        var token = await _tokenStorage.GetTokenAsync();
        if (!string.IsNullOrEmpty(token))
            request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", token);

        return await base.SendAsync(request, cancellationToken);
    }
}
