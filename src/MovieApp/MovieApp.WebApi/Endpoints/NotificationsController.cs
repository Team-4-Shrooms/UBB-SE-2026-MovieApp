using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using MovieApp.Logic.Interfaces.Services;

namespace MovieApp.WebApi.Endpoints;

[Authorize]
[ApiController]
[Route("api/notifications")]
public sealed class NotificationsController : ControllerBase
{
    private readonly INotificationService _notificationService;

    public NotificationsController(INotificationService notificationService)
    {
        _notificationService = notificationService;
    }

    [HttpGet("{userId:int}")]
    public async Task<IActionResult> GetNotificationsByUser(int userId)
    {
        var notifications = await _notificationService.GetNotificationsByUserAsync(userId);
        return Ok(notifications);
    }

    [HttpGet("{userId:int}/unread")]
    public async Task<IActionResult> GetUnreadNotifications(int userId)
    {
        var unreadNotifications = await _notificationService.GetUnreadAsync(userId);
        return Ok(unreadNotifications);
    }

    [HttpDelete("{notificationId:int}")]
    public async Task<IActionResult> RemoveNotification(int notificationId)
    {
        await _notificationService.RemoveNotificationAsync(notificationId);
        return Ok();
    }

    [HttpPost("{notificationId:int}/read")]
    public async Task<IActionResult> MarkRead(int notificationId)
    {
        await _notificationService.MarkReadAsync(notificationId);
        return Ok();
    }

    [HttpPost("{notificationId:int}/mark-read-or-remove")]
    public async Task<IActionResult> MarkReadOrRemove(int notificationId)
    {
        await _notificationService.MarkAsReadOrRemoveAsync(notificationId);
        return Ok();
    }

    [HttpPost("read-all/{userId:int}")]
    public async Task<IActionResult> MarkAllRead(int userId)
    {
        await _notificationService.MarkAllReadAsync(userId);
        return Ok();
    }

    [HttpPost("generate/price-drop")]
    public async Task<IActionResult> GeneratePriceDropNotification([FromBody] GeneratePriceDropNotificationRequest request)
    {
        await _notificationService.GeneratePriceDropNotificationAsync(request.EventId, request.EventTitle);
        return Ok();
    }

    [HttpPost("generate/seats-available")]
    public async Task<IActionResult> GenerateSeatsAvailableNotification([FromBody] GenerateSeatsAvailableNotificationRequest request)
    {
        await _notificationService.GenerateSeatsAvailableNotificationAsync(request.EventId, request.EventTitle);
        return Ok();
    }

    [HttpPost("notify/price-drop")]
    public async Task<IActionResult> NotifyPriceDrop([FromBody] NotifyPriceDropRequest request)
    {
        await _notificationService.NotifyPriceDropAsync(request.EventId, request.OldPrice, request.NewPrice);
        return Ok();
    }

    [HttpPost("notify/seats-available")]
    public async Task<IActionResult> NotifySeatsAvailable([FromBody] NotifySeatsAvailableRequest request)
    {
        await _notificationService.NotifySeatsAvailableAsync(request.EventId, request.NewCapacity);
        return Ok();
    }
}

public sealed record GeneratePriceDropNotificationRequest(int EventId, string EventTitle);

public sealed record GenerateSeatsAvailableNotificationRequest(int EventId, string EventTitle);

public sealed record NotifyPriceDropRequest(int EventId, decimal OldPrice, decimal NewPrice);

public sealed record NotifySeatsAvailableRequest(int EventId, int NewCapacity);
