using LostAndFound.Infrastructure.Services;
using Microsoft.AspNetCore.Mvc;

namespace LostAndFound.Api.Controllers;

[Route("api/notifications")]
public class NotificationsController : ApiControllerBase
{
    private readonly INotificationService _notifications;

    public NotificationsController(INotificationService notifications) => _notifications = notifications;

    [HttpGet]
    public async Task<IActionResult> Mine() =>
        Ok(await _notifications.GetMineAsync(CurrentUserId!));

    [HttpPost("{id:int}/read")]
    public async Task<IActionResult> MarkRead(int id) =>
        ToAction(await _notifications.MarkReadAsync(id, CurrentUserId!));
}
