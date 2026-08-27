using LostAndFound.Infrastructure.Services;
using LostAndFound.Shared.Dtos.Items;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace LostAndFound.Api.Controllers;

public class ItemsController : ApiControllerBase
{
    private readonly IItemService _items;

    public ItemsController(IItemService items) => _items = items;

    [HttpGet]
    [AllowAnonymous]
    public async Task<IActionResult> Search([FromQuery] ItemSearchParameters parameters) =>
        ToAction(await _items.SearchAsync(parameters));

    [HttpGet("{id:int}")]
    [AllowAnonymous]
    public async Task<IActionResult> Get(int id) => ToAction(await _items.GetByIdAsync(id));

    [HttpPost]
    public async Task<IActionResult> Create(CreateItemRequest request) =>
        ToAction(await _items.CreateAsync(request, CurrentUserId!));

    [HttpPut("{id:int}")]
    public async Task<IActionResult> Update(int id, UpdateItemRequest request) =>
        ToAction(await _items.UpdateAsync(id, request, CurrentUserId!));

    [HttpDelete("{id:int}")]
    public async Task<IActionResult> Delete(int id) =>
        ToAction(await _items.DeleteAsync(id, CurrentUserId!));
}
