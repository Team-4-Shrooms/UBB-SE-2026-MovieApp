namespace MovieApp.DataLayer.Interfaces.Repositories;

using System.Collections.Generic;
using System.Threading.Tasks;
using MovieApp.DataLayer.Models;

public interface IPriceWatcherRepository
{
    Task<List<PriceWatcher>> GetAllWatchedEventsAsync();

    Task<bool> AddWatchAsync(PriceWatcher watchedEvent);

    Task RemoveWatchAsync(int eventIdentifier);

    Task<PriceWatcher?> GetWatchAsync(int eventIdentifier);

    Task<bool> IsWatchingAsync(int eventIdentifier);
}
