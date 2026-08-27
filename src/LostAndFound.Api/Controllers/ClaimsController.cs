using LostAndFound.Domain.Constants;
using LostAndFound.Infrastructure.Services;
using LostAndFound.Shared.Dtos.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace LostAndFound.Api.Controllers;

[Route("api")]
public class ClaimsController : ApiControllerBase
{
    private readonly IClaimService _claims;

    public ClaimsController(IClaimService claims) => _claims = claims;

    [HttpPost("items/{itemId:int}/claims")]
    public async Task<IActionResult> Create(int itemId, CreateClaimRequest request) =>
        ToAction(await _claims.CreateAsync(itemId, request, CurrentUserId!));

    [HttpGet("items/{itemId:int}/claims")]
    [Authorize(Roles = Roles.Staff + "," + Roles.Admin)]
    public async Task<IActionResult> ForItem(int itemId) =>
        ToAction(await _claims.GetForItemAsync(itemId));

    [HttpGet("claims/mine")]
    public async Task<IActionResult> Mine() =>
        ToAction(await _claims.GetMineAsync(CurrentUserId!));

    [HttpPost("claims/{id:int}/review")]
    [Authorize(Roles = Roles.Staff + "," + Roles.Admin)]
    public async Task<IActionResult> Review(int id, ReviewClaimRequest request) =>
        ToAction(await _claims.ReviewAsync(id, request, CurrentUserId!));
}
