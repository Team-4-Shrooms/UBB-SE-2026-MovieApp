using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using MovieApp.DataLayer.Models;
using MovieApp.Logic.Interfaces.Services;

namespace MovieApp.Web.Controllers;

[Microsoft.AspNetCore.Authorization.Authorize]
public sealed class NotificationController : Controller
{
    private readonly INotificationService _notificationService;
    private readonly ICurrentUserService _currentUserService;

    public NotificationController(INotificationService notificationService, ICurrentUserService currentUserService)
    {
        _notificationService = notificationService;
        _currentUserService = currentUserService;
    }

    [HttpGet]
    public async Task<IActionResult> Index()
    {
        int userId = _currentUserService.UserId;

        IReadOnlyList<Notification> notifications = await _notificationService.GetNotificationsByUserAsync(userId);

        ViewData["UnreadCount"] = await _notificationService.GetUnreadCountAsync(userId);

        return View(notifications.OrderByDescending(notification => notification.CreatedAt));
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> MarkRead(int id)
    {
        await _notificationService.MarkReadAsync(id);

        return RedirectToAction(nameof(Index));
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> MarkAllRead()
    {
        int userId = _currentUserService.UserId;
        await _notificationService.MarkAllReadAsync(userId);

        return RedirectToAction(nameof(Index));
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Delete(int id)
    {
        await _notificationService.RemoveNotificationAsync(id);

        return RedirectToAction(nameof(Index));
    }
}

