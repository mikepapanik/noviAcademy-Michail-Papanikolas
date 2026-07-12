using NLog;
using WorldRank.Application.Interfaces;
using WorldRank.Domain.Enums;
using WorldRank.Domain.Exceptions;
using WorldRank.Domain.Wallets;

namespace WorldRank.Infrastructure.Repositories
{
    public class InMemoryWalletRepository : IWalletRepository
    {
        private static readonly Logger _logger =
            LogManager.GetCurrentClassLogger();

        private readonly List<Wallet> _wallets =
            new List<Wallet>();

        public void Add(Wallet wallet)
        {
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

            _logger.Info(
                "Wallet created for player {PlayerId} in {Currency} with balance {Balance}",
                wallet.PlayerId,
                wallet.Currency,
                wallet.Balance);
        }

        public Wallet GetWallet(
            int playerId,
            Currency currency)
        {
            var wallet = _wallets.FirstOrDefault(item =>
                item.PlayerId == playerId &&
                item.Currency == currency);

            if (wallet is null)
            {
                throw new WalletNotFoundException(
                    playerId,
                    currency);
            }

            return wallet;
        }

        public Wallet[] GetAll()
        {
            return _wallets.ToArray();
        }

        public List<Wallet> GetAllWalletsByPlayerId(
            int playerId)
        {
            return _wallets
                .Where(item => item.PlayerId == playerId)
                .ToList();
        }

        public void UpdateBalance(
            int playerId,
            Currency currency,
            decimal newBalance)
        {
            var wallet = GetWallet(
                playerId,
                currency);

            wallet.SetBalance(newBalance);

            _logger.Info(
                "Wallet balance updated for player {PlayerId} in {Currency}. New balance {Balance}",
                playerId,
                currency,
                newBalance);
        }

        public void Deposit(
            int playerId,
            Currency currency,
            decimal amount)
        {
            var wallet = GetWallet(
                playerId,
                currency);

            wallet.Deposit(amount);

            _logger.Info(
                "Deposit completed for player {PlayerId} in {Currency}. Amount {Amount}",
                playerId,
                currency,
                amount);
        }

        public void Withdraw(
            int playerId,
            Currency currency,
            decimal amount)
        {
            var wallet = GetWallet(
                playerId,
                currency);

            wallet.Withdraw(amount);

            _logger.Info(
                "Withdraw completed for player {PlayerId} in {Currency}. Amount {Amount}",
                playerId,
                currency,
                amount);
        }

        public void Block(
            int playerId,
            Currency currency)
        {
            var wallet = GetWallet(
                playerId,
                currency);

            wallet.Block();

            _logger.Info(
                "Wallet blocked for player {PlayerId} in {Currency}",
                playerId,
                currency);
        }

        public void Unblock(
            int playerId,
            Currency currency)
        {
            var wallet = GetWallet(
                playerId,
                currency);

            wallet.Unblock();

            _logger.Info(
                "Wallet unblocked for player {PlayerId} in {Currency}",
                playerId,
                currency);
        }

        public void SaveChanges()
        {
            // No persistence is required for the in-memory implementation.
        }
    }
}