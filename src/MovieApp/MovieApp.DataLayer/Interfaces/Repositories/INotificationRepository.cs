// <copyright file="INotificationRepository.cs" company="MovieApp">
// Copyright (c) MovieApp. All rights reserved.
// </copyright>

namespace MovieApp.DataLayer.Interfaces.Repositories;

using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using MovieApp.DataLayer.Models;

public interface INotificationRepository
{
    Task AddAsync(Notification notification, CancellationToken cancellationToken = default);

    Task RemoveAsync(int notificationIdentifier, CancellationToken cancellationToken = default);

    Task<IReadOnlyList<Notification>> FindByUserAsync(int userIdentifier, CancellationToken cancellationToken = default);
    Task<IReadOnlyList<Notification>> GetUnreadByUserAsync(int userIdentifier, CancellationToken cancellationToken = default);
    Task MarkReadAsync(int notificationIdentifier, CancellationToken cancellationToken = default);
    Task MarkAllReadAsync(int userIdentifier, CancellationToken cancellationToken = default);
}
