using WorldRank.Application.Interfaces;
using WorldRank.Domain.Enums;
using WorldRank.Domain.Exceptions;
using WorldRank.Domain.Wallets;

namespace WorldRank.Infrastructure.Repositories
{
    public class InMemoryWalletRepository : IWalletRepository
    {

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


        }

        public void Block(
            int playerId,
            Currency currency)
        {
            var wallet = GetWallet(
                playerId,
                currency);

            wallet.Block();


        }

        public void Unblock(
            int playerId,
            Currency currency)
        {
            var wallet = GetWallet(
                playerId,
                currency);

            wallet.Unblock();


        }

        public void Update(Wallet wallet)
        {
            var index = _wallets.FindIndex(item =>
                item.Id == wallet.Id);

            if (index >= 0)
            {
                _wallets[index] = wallet;
            }
        }
    }
}