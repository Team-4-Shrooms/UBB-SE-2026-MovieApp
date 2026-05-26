using System.Collections.Generic;
using System.Threading.Tasks;
using MovieApp.DataLayer.Interfaces.Repositories;
using MovieApp.DataLayer.Models;
using MovieApp.Logic.Interfaces.Services;

namespace MovieApp.Logic.Services
{
    public class PriceWatcherService : IPriceWatcherService
    {
        private readonly IPriceWatcherRepository _priceWatcherRepository;

        public PriceWatcherService(IPriceWatcherRepository priceWatcherRepository)
        {
            _priceWatcherRepository = priceWatcherRepository;
        }

        public Task<List<PriceWatcher>> GetAllWatchedEventsAsync() =>
            _priceWatcherRepository.GetAllWatchedEventsAsync();

        public Task<bool> AddWatchAsync(PriceWatcher watchedEvent) =>
            _priceWatcherRepository.AddWatchAsync(watchedEvent);

        public Task RemoveWatchAsync(int eventIdentifier) =>
            _priceWatcherRepository.RemoveWatchAsync(eventIdentifier);

        public Task<PriceWatcher?> GetWatchAsync(int eventIdentifier) =>
            _priceWatcherRepository.GetWatchAsync(eventIdentifier);

        public Task<bool> IsWatchingAsync(int eventIdentifier) =>
            _priceWatcherRepository.IsWatchingAsync(eventIdentifier);
    }
}
