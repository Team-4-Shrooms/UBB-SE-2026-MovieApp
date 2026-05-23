using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using MovieApp.Logic.Interfaces.Services;
using MovieApp.WebApi.Filters;
using MovieApp.WebApi.Mappings;
using MovieApp.WebDTOs.DTOs.RequestDTOs;

namespace MovieApp.WebApi.Endpoints;

[Authorize]
[ApiController]
[Route("api/notifications")]
public sealed class NotificationsEndpointsController : ControllerBase
{
    private readonly INotificationService _notificationService;

    public NotificationsEndpointsController(INotificationService notificationService)
    {
        this._notificationService = notificationService;
    }

    [HttpGet("{userId:int}")]
    [RequireMatchingUser]
    public async Task<IActionResult> GetNotificationsByUser(int userId)
    {
        var notifications = await this._notificationService.GetNotificationsByUserAsync(userId);
        return Ok(notifications.Select(notification => notification.ToDto()));
    }

    [HttpGet("{userId:int}/unread")]
    [RequireMatchingUser]
    public async Task<IActionResult> GetUnreadNotifications(int userId)
    {
        var unreadNotifications = await this._notificationService.GetUnreadNotificationsAsync(userId);
        return Ok(unreadNotifications.Select(notification => notification.ToDto()));
    }

    [HttpDelete("{notificationId:int}")]
    public async Task<IActionResult> RemoveNotification(int notificationId)
    {
        await this._notificationService.RemoveNotificationAsync(notificationId);
        return Ok();
    }

    [HttpPost("{notificationId:int}/read")]
    public async Task<IActionResult> MarkRead(int notificationId)
    {
        await this._notificationService.MarkReadAsync(notificationId);
        return Ok();
    }

    [HttpPost("{notificationId:int}/mark-read-or-remove")]
    public async Task<IActionResult> MarkReadOrRemove(int notificationId)
    {
        await this._notificationService.MarkAsReadOrRemoveAsync(notificationId);
        return Ok();
    }

    [HttpPost("read-all/{userId:int}")]
    [RequireMatchingUser]
    public async Task<IActionResult> MarkAllRead(int userId)
    {
        await this._notificationService.MarkAllReadAsync(userId);
        return Ok();
    }

    [HttpPost("generate/price-drop")]
    public async Task<IActionResult> GeneratePriceDropNotification([FromBody] GeneratePriceDropNotificationRequestBody requestBody)
    {
        await this._notificationService.GeneratePriceDropNotificationAsync(requestBody.EventId, requestBody.EventTitle);
        return Ok();
    }

    [HttpPost("generate/seats-available")]
    public async Task<IActionResult> GenerateSeatsAvailableNotification([FromBody] GenerateSeatsAvailableNotificationRequestBody requestBody)
    {
        await this._notificationService.GenerateSeatsAvailableNotificationAsync(requestBody.EventId, requestBody.EventTitle);
        return Ok();
    }

    [HttpPost("notify/price-drop")]
    public async Task<IActionResult> NotifyPriceDrop([FromBody] NotifyPriceDropRequestBody requestBody)
    {
        await this._notificationService.NotifyPriceDropAsync(requestBody.EventId, requestBody.OldPrice, requestBody.NewPrice);
        return Ok();
    }

    [HttpPost("notify/seats-available")]
    public async Task<IActionResult> NotifySeatsAvailable([FromBody] NotifySeatsAvailableRequestBody requestBody)
    {
        await this._notificationService.NotifySeatsAvailableAsync(requestBody.EventId, requestBody.NewCapacity);
        return Ok();
    }
}
