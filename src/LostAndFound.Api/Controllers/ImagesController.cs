using LostAndFound.Infrastructure.Services;
using LostAndFound.Shared.Dtos;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Routing;

namespace LostAndFound.Api.Controllers;

[ApiController]
[Route("api/items/{itemId:int}/images")]
public class ImagesController : ControllerBase
{
    private readonly IItemImageService _images;

    public ImagesController(IItemImageService images) => _images = images;

    [HttpGet]
    public async Task<IActionResult> GetForItem(int itemId) =>
        ToAction(await _images.GetForItemAsync(itemId));

    [HttpPost]
    [Authorize]
    public async Task<IActionResult> Upload(int itemId, IFormFile file, [FromQuery] bool isPrimary = false)
    {
        if (file is null || file.Length == 0)
            return BadRequest(new { error = "No file provided." });

        await using var stream = file.OpenReadStream();
        return ToAction(await _images.UploadAsync(itemId, file.FileName, stream, isPrimary));
    }

    [HttpDelete("{imageId:int}")]
    [Authorize]
    public async Task<IActionResult> Delete(int itemId, int imageId) =>
        ToAction(await _images.DeleteAsync(itemId, imageId));

    private IActionResult ToAction(Result result) =>
        result.Succeeded ? Ok(new { success = true }) : StatusCode(result.StatusCode, new { error = result.Error });

    private IActionResult ToAction<T>(Result<T> result) =>
        result.Succeeded ? Ok(result.Data) : StatusCode(result.StatusCode, new { error = result.Error });
}
