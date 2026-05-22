// <copyright file="INotificationService.cs" company="MovieApp">
// Copyright (c) MovieApp. All rights reserved.
// </copyright>

namespace MovieApp.Logic.Interfaces.Services;

using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using MovieApp.DataLayer.Models;

public interface INotificationService
{
    Task GeneratePriceDropNotificationAsync(int eventIdentifier, string eventTitle, CancellationToken cancellationToken = default);

    Task GenerateSeatsAvailableNotificationAsync(int eventIdentifier, string eventTitle, CancellationToken cancellationToken = default);

    Task<IReadOnlyList<Notification>> GetNotificationsByUserAsync(int userIdentifier, CancellationToken cancellationToken = default);

    Task RemoveNotificationAsync(int notificationIdentifier, CancellationToken cancellationToken = default);

    Task NotifyPriceDropAsync(int eventIdentifier, decimal oldPrice, decimal newPrice, CancellationToken cancellationToken = default);

    Task NotifySeatsAvailableAsync(int eventIdentifier, int newCapacity, CancellationToken cancellationToken = default);

    Task MarkAsReadOrRemoveAsync(int notificationIdentifier, CancellationToken cancellationToken = default);
    Task<IReadOnlyList<Notification>> GetUnreadNotificationsAsync(int userIdentifier, CancellationToken cancellationToken = default);
    Task MarkReadAsync(int notificationIdentifier, CancellationToken cancellationToken = default);
    Task MarkAllReadAsync(int userIdentifier, CancellationToken cancellationToken = default);
}
