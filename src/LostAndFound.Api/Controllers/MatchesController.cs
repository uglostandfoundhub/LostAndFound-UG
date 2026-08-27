using LostAndFound.Infrastructure.Services;
using Microsoft.AspNetCore.Mvc;

namespace LostAndFound.Api.Controllers;

[Route("api")]
public class MatchesController : ApiControllerBase
{
    private readonly IMatchService _matches;

    public MatchesController(IMatchService matches) => _matches = matches;

    [HttpGet("matches")]
    public async Task<IActionResult> Mine() =>
        ToAction(await _matches.GetForUserAsync(CurrentUserId!));

    [HttpPost("items/{itemId:int}/matches")]
    public async Task<IActionResult> Generate(int itemId) =>
        ToAction(await _matches.GenerateForItemAsync(itemId, CurrentUserId!));

    [HttpPost("matches/{id:int}/dismiss")]
    public async Task<IActionResult> Dismiss(int id) =>
        ToAction(await _matches.DismissAsync(id, CurrentUserId!));
}
