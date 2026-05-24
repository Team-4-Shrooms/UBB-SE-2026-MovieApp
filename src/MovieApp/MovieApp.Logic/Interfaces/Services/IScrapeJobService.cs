using System.Collections.Generic;
using System.Threading.Tasks;
using MovieApp.DataLayer.Models;

namespace MovieApp.Logic.Interfaces.Services
{
    public interface IScrapeJobService
    {
        Task<int> CreateJobAsync(ScrapeJob job);
        Task<ScrapeJob?> GetJobByIdAsync(int jobId);
        Task UpdateJobAsync(ScrapeJob job);
        Task AddLogEntryAsync(ScrapeJobLog log);
        Task<List<ScrapeJob>> GetAllJobsAsync();
        Task<List<ScrapeJobLog>> GetLogsForJobAsync(int jobId);
        Task<List<ScrapeJobLog>> GetAllLogsAsync();
        Task<DashboardStats> GetDashboardStatsAsync();
        Task<List<Movie>> SearchMoviesByNameAsync(string partialName);
        Task<int?> FindMovieByTitleAsync(string title);
        Task<bool> ReelExistsByVideoUrlAsync(string videoUrl);
        Task<int> InsertScrapedReelAsync(Reel reel);
        Task<List<Movie>> GetAllMoviesAsync();
        Task<List<Reel>> GetAllReelsAsync();
    }
}
