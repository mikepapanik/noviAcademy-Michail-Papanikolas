using WorldRank.Application.Interfaces;
using WorldRank.Domain.Enums;
using WorldRank.Domain.Exceptions;
using WorldRank.Domain.Wallets;

namespace WorldRank.Infrastructure.Repositories
{
    public class InMemoryWalletRepository : IWalletRepository
    {
        private readonly List<Wallet> _wallets = [];

        public Task AddAsync(Wallet wallet,CancellationToken cancellationToken)
        {
            cancellationToken.ThrowIfCancellationRequested();

            var exists = _wallets.Any(item =>
                item.PlayerId == wallet.PlayerId &&
                item.Currency == wallet.Currency);

            if (exists)
            {
                throw new DuplicateWalletException(
                    wallet.PlayerId,
                    wallet.Currency);
            }

            _wallets.Add(wallet);

            return Task.CompletedTask;
        }

        public Task<Wallet?> GetByIdAsync(int walletId,CancellationToken cancellationToken)
        {
            cancellationToken.ThrowIfCancellationRequested();

            var wallet = _wallets.FirstOrDefault(
                item => item.Id == walletId);

            return Task.FromResult(wallet);
        }

        public Task<Wallet?> GetWalletAsync(int playerId,Currency currency,CancellationToken cancellationToken)
        {
            cancellationToken.ThrowIfCancellationRequested();

            var wallet = _wallets.FirstOrDefault(item =>
                item.PlayerId == playerId &&
                item.Currency == currency);

            return Task.FromResult(wallet);
        }

        public Task<IReadOnlyList<Wallet>> GetAllAsync(CancellationToken cancellationToken)
        {
            cancellationToken.ThrowIfCancellationRequested();

            IReadOnlyList<Wallet> wallets =
                _wallets.ToList();

            return Task.FromResult(wallets);
        }

        public Task<IReadOnlyList<Wallet>>
            GetAllWalletsByPlayerIdAsync(int playerId,CancellationToken cancellationToken)
        {
            cancellationToken.ThrowIfCancellationRequested();

            IReadOnlyList<Wallet> wallets = _wallets
                .Where(item =>
                    item.PlayerId == playerId)
                .ToList();

            return Task.FromResult(wallets);
        }

        public Task UpdateAsync(Wallet wallet,CancellationToken cancellationToken)
        {
            cancellationToken.ThrowIfCancellationRequested();

            var index = _wallets.FindIndex(
                item => item.Id == wallet.Id);

            if (index >= 0)
            {
                _wallets[index] = wallet;
            }

            return Task.CompletedTask;
        }
    }
}