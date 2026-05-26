namespace MovieApp.Logic.Services
{
    using System.Collections.Generic;
    using System.Threading;
    using System.Threading.Tasks;
    using MovieApp.DataLayer.Interfaces.Repositories;
    using MovieApp.DataLayer.Models;
    using MovieApp.Logic.Interfaces.Services;

    public sealed class AmbassadorService : IAmbassadorService
    {
        private readonly IAmbassadorRepository _repository;

        public AmbassadorService(IAmbassadorRepository repository)
        {
            _repository = repository;
        }

        public Task<bool> IsReferralCodeValidAsync(string referralCode, CancellationToken cancellationToken = default)
        {
            return _repository.IsReferralCodeValidAsync(referralCode, cancellationToken);
        }

        public Task<IEnumerable<AmbassadorProfile>> GetAllAmbassadorsAsync(CancellationToken cancellationToken = default)
        {
            return _repository.GetAllAmbassadorsAsync(cancellationToken);
        }

        public Task<AmbassadorProfile?> GetAmbassadorByIdAsync(int id, CancellationToken cancellationToken = default)
        {
            return _repository.GetAmbassadorByIdAsync(id, cancellationToken);
        }

        public Task<string?> GetReferralCodeAsync(int userId, CancellationToken cancellationToken = default)
        {
            return _repository.GetReferralCodeAsync(userId, cancellationToken);
        }

        public Task CreateAmbassadorProfileAsync(int userId, string referralCode, CancellationToken cancellationToken = default)
        {
            return _repository.CreateAmbassadorProfileAsync(userId, referralCode, cancellationToken);
        }

        public async Task ProcessReferralAsync(string referralCode, int friendId, int eventId, CancellationToken cancellationToken = default)
        {
            int? ambassadorId = await _repository.GetUserIdByReferralCodeAsync(referralCode, cancellationToken);
            if (ambassadorId is null)
            {
                return;
            }

            bool isDuplicate = await _repository.HasReferralLogAsync(ambassadorId.Value, friendId, eventId, cancellationToken);
            if (isDuplicate)
            {
                return;
            }

            await _repository.AddReferralLogAsync(ambassadorId.Value, friendId, eventId, cancellationToken);
            await _repository.IncrementRewardBalanceAsync(ambassadorId.Value, cancellationToken);
        }

        public Task<IEnumerable<ReferralHistoryItem>> GetReferralHistoryAsync(int ambassadorId, CancellationToken cancellationToken = default)
        {
            return _repository.GetReferralHistoryAsync(ambassadorId, cancellationToken);
        }

        public Task<int> GetRewardBalanceAsync(int userId, CancellationToken cancellationToken = default)
        {
            return _repository.GetRewardBalanceAsync(userId, cancellationToken);
        }

        public async Task RedeemRewardAsync(int userId, CancellationToken cancellationToken = default)
        {
            int balance = await _repository.GetRewardBalanceAsync(userId, cancellationToken);
            if (balance > 0)
            {
                await _repository.DecrementRewardBalanceAsync(userId, cancellationToken);
            }
        }

        public Task<int?> ResolveCodeToUserIdAsync(string referralCode, CancellationToken cancellationToken = default)
        {
            return _repository.GetUserIdByReferralCodeAsync(referralCode, cancellationToken);
        }

        public Task<bool> ReferralLogExistsAsync(int ambassadorId, int friendId, int eventId, CancellationToken cancellationToken = default)
        {
            return _repository.HasReferralLogAsync(ambassadorId, friendId, eventId, cancellationToken);
        }

        public Task LogReferralByAmbassadorIdAsync(int ambassadorId, int friendId, int eventId, CancellationToken cancellationToken = default)
        {
            return _repository.AddReferralLogAsync(ambassadorId, friendId, eventId, cancellationToken);
        }

        public Task DecrementRewardBalanceAsync(int userId, CancellationToken cancellationToken = default)
        {
            return _repository.DecrementRewardBalanceAsync(userId, cancellationToken);
        }
    }
}
