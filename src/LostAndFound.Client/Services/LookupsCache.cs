using LostAndFound.Shared.Dtos;

namespace LostAndFound.Client.Services;

public class LookupsCache
{
    private readonly ApiClient _api;
    private IReadOnlyList<CategoryDto>? _categories;
    private IReadOnlyList<LocationDto>? _locations;

    public LookupsCache(ApiClient api) => _api = api;

    public async Task<IReadOnlyList<CategoryDto>> GetCategoriesAsync()
    {
        _categories ??= await _api.GetCategoriesAsync() ?? Array.Empty<CategoryDto>();
        return _categories;
    }

    public async Task<IReadOnlyList<LocationDto>> GetLocationsAsync()
    {
        _locations ??= await _api.GetLocationsAsync() ?? Array.Empty<LocationDto>();
        return _locations;
    }

    public string CategoryName(int id) => _categories?.FirstOrDefault(c => c.Id == id)?.Name ?? string.Empty;
    public string LocationName(int id) => _locations?.FirstOrDefault(l => l.Id == id)?.Name ?? string.Empty;
}
