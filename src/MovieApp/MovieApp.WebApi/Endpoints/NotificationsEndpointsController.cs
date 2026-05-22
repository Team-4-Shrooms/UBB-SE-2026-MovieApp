using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using MovieApp.Logic.Interfaces.Services;
using MovieApp.WebApi.Mappings;
using MovieApp.WebDTOs.DTOs.RequestDTOs;
using System.Security.Claims;

namespace MovieApp.WebApi.Endpoints;

[Authorize]
[ApiController]
[Route("api/notifications")]
public sealed class NotificationsEndpointsController : ControllerBase
{
    private readonly INotificationService _notificationService;

    public NotificationsEndpointsController(INotificationService notificationService)
    {
        _notificationService = notificationService;
    }

    private bool TryGetCurrentUserId(out int userId)
    {
        userId = default;
        if (User?.Identity == null || !User.Identity.IsAuthenticated)
            {
            return false;
        }

        // Try standard claim types, fall back to common JWT claim names
        var idClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value
                      ?? User.FindFirst("id")?.Value
                      ?? User.FindFirst("sub")?.Value;

        if (string.IsNullOrWhiteSpace(idClaim))
        {
            return false;
        }

        return int.TryParse(idClaim, out userId);
    }

    private ActionResult? EnsureUserMatches(int userId)
    {
        if (!TryGetCurrentUserId(out int currentUserId))
        {
            return Unauthorized();
        }

        if (currentUserId != userId)
            {
            return Forbid();
        }

        return null;
    }

    [HttpGet("{userId:int}")]
    public async Task<IActionResult> GetNotificationsByUser(int userId)
    {
        var check = EnsureUserMatches(userId);
        if (check is not null) {
            return check;
        }

        var notifications = await _notificationService.GetNotificationsByUserAsync(userId);
        return Ok(notifications.Select(notification => notification.ToDto()));
    }

    [HttpGet("{userId:int}/unread")]
    public async Task<IActionResult> GetUnreadNotifications(int userId)
    {
        var check = EnsureUserMatches(userId);
        if (check is not null) {
            return check;
        }

        var unreadNotifications = await _notificationService.GetUnreadNotificationsAsync(userId);
        return Ok(unreadNotifications.Select(notification => notification.ToDto()));
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
        var check = EnsureUserMatches(userId);
        if (check is not null) {
            return check;
        }

        await _notificationService.MarkAllReadAsync(userId);
        return Ok();
    }

    [HttpPost("generate/price-drop")]
    public async Task<IActionResult> GeneratePriceDropNotification([FromBody] GeneratePriceDropNotificationRequestBody requestBody)
    {
        await _notificationService.GeneratePriceDropNotificationAsync(requestBody.EventId, requestBody.EventTitle);
        return Ok();
    }

    [HttpPost("generate/seats-available")]
    public async Task<IActionResult> GenerateSeatsAvailableNotification([FromBody] GenerateSeatsAvailableNotificationRequestBody requestBody)
    {
        await _notificationService.GenerateSeatsAvailableNotificationAsync(requestBody.EventId, requestBody.EventTitle);
        return Ok();
    }

    [HttpPost("notify/price-drop")]
    public async Task<IActionResult> NotifyPriceDrop([FromBody] NotifyPriceDropRequestBody requestBody)
    {
        await _notificationService.NotifyPriceDropAsync(requestBody.EventId, requestBody.OldPrice, requestBody.NewPrice);
        return Ok();
    }

    [HttpPost("notify/seats-available")]
    public async Task<IActionResult> NotifySeatsAvailable([FromBody] NotifySeatsAvailableRequestBody requestBody)
    {
        await _notificationService.NotifySeatsAvailableAsync(requestBody.EventId, requestBody.NewCapacity);
        return Ok();
    }
}
