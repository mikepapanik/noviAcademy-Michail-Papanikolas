using WorldRank.Domain.Enums;
using WorldRank.Domain.Wallets;

namespace WorldRank.Application.Interfaces
{
    public interface IWalletRepository
    {
        void Add(Wallet wallet);

        Wallet GetWallet(int playerId, Currency currency);

        List<Wallet> GetAllWalletsByPlayerId(int playerId);

        void UpdateBalance(int playerId, Currency currency, decimal newBalance);

        void Deposit(int playerId, Currency currency, decimal amount);

        void Withdraw(int playerId, Currency currency, decimal amount);

        void Block(int playerId, Currency currency);

        void Unblock(int playerId, Currency currency);
    }
}