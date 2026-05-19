namespace MovieApp.DataLayer.Models;

using System;

public enum NotificationState
{
    /// <summary>
    /// The notification has not been viewed by the user.
    /// </summary>
    Unread = 0,

    /// <summary>
    /// The notification has been acknowledged or viewed by the user.
    /// </summary>
    Read = 1,
}
