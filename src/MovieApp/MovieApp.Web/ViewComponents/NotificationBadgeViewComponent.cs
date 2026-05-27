using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using MovieApp.Logic.Interfaces.Services;

namespace MovieApp.Web.ViewComponents;

public sealed class NotificationBadgeViewComponent : ViewComponent
{
    private readonly INotificationService _notificationService;
    private readonly ICurrentUserService _currentUserService;

    public NotificationBadgeViewComponent(INotificationService notificationService, ICurrentUserService currentUserService)
    {
        _notificationService = notificationService;
        _currentUserService = currentUserService;
    }

    public async Task<IViewComponentResult> InvokeAsync()
    {
        int userId = _currentUserService.UserId;
        int unreadCount = await _notificationService.GetUnreadCountAsync(userId);
        return View(unreadCount);
    }
}
