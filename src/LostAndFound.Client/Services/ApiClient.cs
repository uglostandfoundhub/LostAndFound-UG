using System.Net.Http.Json;
using LostAndFound.Shared.Dtos;
using LostAndFound.Shared.Dtos.Auth;
using LostAndFound.Shared.Dtos.Claims;
using LostAndFound.Shared.Dtos.Items;
using LostAndFound.Shared.Dtos.Matches;
using LostAndFound.Shared.Dtos.Notifications;

namespace LostAndFound.Client.Services;

public class ApiClient
{
    private readonly HttpClient _http;

    public ApiClient(HttpClient http) => _http = http;

    public string? BaseAddress => _http.BaseAddress?.ToString();

    // ----- Auth -----
    public async Task<AuthResponse?> LoginAsync(LoginRequest request)
    {
        var resp = await _http.PostAsJsonAsync("api/auth/login", request);
        return resp.IsSuccessStatusCode ? await resp.Content.ReadFromJsonAsync<AuthResponse>() : null;
    }

    public async Task<AuthResponse?> RegisterAsync(RegisterRequest request)
    {
        var resp = await _http.PostAsJsonAsync("api/auth/register", request);
        return resp.IsSuccessStatusCode ? await resp.Content.ReadFromJsonAsync<AuthResponse>() : null;
    }

    public async Task<UserDto?> GetCurrentUserAsync()
    {
        var resp = await _http.GetAsync("api/auth/me");
        return resp.IsSuccessStatusCode ? await resp.Content.ReadFromJsonAsync<UserDto>() : null;
    }

    // ----- Items -----
    public async Task<PagedResult<ItemDto>?> GetItemsAsync(ItemSearchParameters p) =>
        await _http.GetFromJsonAsync<PagedResult<ItemDto>>($"api/items{ToQuery(p)}");

    public async Task<ItemDto?> GetItemAsync(int id) =>
        await _http.GetFromJsonAsync<ItemDto>($"api/items/{id}");

    public async Task<ItemDto?> CreateItemAsync(CreateItemRequest request)
    {
        var resp = await _http.PostAsJsonAsync("api/items", request);
        return resp.IsSuccessStatusCode ? await resp.Content.ReadFromJsonAsync<ItemDto>() : null;
    }

    public async Task<bool> UpdateItemAsync(int id, UpdateItemRequest request)
    {
        var resp = await _http.PutAsJsonAsync($"api/items/{id}", request);
        return resp.IsSuccessStatusCode;
    }

    public async Task<bool> DeleteItemAsync(int id)
    {
        var resp = await _http.DeleteAsync($"api/items/{id}");
        return resp.IsSuccessStatusCode;
    }

    // ----- Lookups -----
    public async Task<IReadOnlyList<CategoryDto>?> GetCategoriesAsync() =>
        await _http.GetFromJsonAsync<IReadOnlyList<CategoryDto>>("api/categories");

    public async Task<IReadOnlyList<LocationDto>?> GetLocationsAsync() =>
        await _http.GetFromJsonAsync<IReadOnlyList<LocationDto>>("api/locations");

    // ----- Claims -----
    public async Task<ClaimDto?> CreateClaimAsync(int itemId, CreateClaimRequest request)
    {
        var resp = await _http.PostAsJsonAsync($"api/items/{itemId}/claims", request);
        return resp.IsSuccessStatusCode ? await resp.Content.ReadFromJsonAsync<ClaimDto>() : null;
    }

    public async Task<IReadOnlyList<ClaimDto>?> GetMyClaimsAsync() =>
        await _http.GetFromJsonAsync<IReadOnlyList<ClaimDto>>("api/claims/mine");

    public async Task<ClaimDto?> ReviewClaimAsync(int id, ReviewClaimRequest request)
    {
        var resp = await _http.PostAsJsonAsync($"api/claims/{id}/review", request);
        return resp.IsSuccessStatusCode ? await resp.Content.ReadFromJsonAsync<ClaimDto>() : null;
    }

    // ----- Matches -----
    public async Task<IReadOnlyList<MatchDto>?> GetMyMatchesAsync() =>
        await _http.GetFromJsonAsync<IReadOnlyList<MatchDto>>("api/matches");

    public async Task<IReadOnlyList<MatchDto>?> GenerateMatchesAsync(int itemId)
    {
        var resp = await _http.PostAsync($"api/items/{itemId}/matches", null);
        return resp.IsSuccessStatusCode ? await resp.Content.ReadFromJsonAsync<IReadOnlyList<MatchDto>>() : null;
    }

    public async Task<bool> DismissMatchAsync(int id)
    {
        var resp = await _http.PostAsync($"api/matches/{id}/dismiss", null);
        return resp.IsSuccessStatusCode;
    }

    // ----- Notifications -----
    public async Task<IReadOnlyList<NotificationDto>?> GetMyNotificationsAsync() =>
        await _http.GetFromJsonAsync<IReadOnlyList<NotificationDto>>("api/notifications");

    public async Task<bool> MarkNotificationReadAsync(int id)
    {
        var resp = await _http.PostAsync($"api/notifications/{id}/read", null);
        return resp.IsSuccessStatusCode;
    }

    private static string ToQuery(ItemSearchParameters p)
    {
        var parts = new List<string>();
        if (p.Type.HasValue) parts.Add($"type={(int)p.Type.Value}");
        if (p.Status.HasValue) parts.Add($"status={(int)p.Status.Value}");
        if (p.CategoryId.HasValue) parts.Add($"categoryId={p.CategoryId.Value}");
        if (p.LocationId.HasValue) parts.Add($"locationId={p.LocationId.Value}");
        if (!string.IsNullOrWhiteSpace(p.SearchTerm)) parts.Add($"searchTerm={Uri.EscapeDataString(p.SearchTerm)}");
        if (p.FromDate.HasValue) parts.Add($"fromDate={p.FromDate.Value:o}");
        if (p.ToDate.HasValue) parts.Add($"toDate={p.ToDate.Value:o}");
        if (p.Page > 1) parts.Add($"page={p.Page}");
        if (p.PageSize != 20) parts.Add($"pageSize={p.PageSize}");
        return parts.Count == 0 ? string.Empty : "?" + string.Join("&", parts);
    }
}
