using WorldRank.Domain.Enums;
using WorldRank.Domain.Exceptions;

namespace WorldRank.Domain.Wallets
{
    public class Wallet : IWallet
    {
        public int Id { get; }

        public int PlayerId { get; }

        public Currency Currency { get; }

        public decimal Balance { get; private set; }

        public bool IsBlocked { get; private set; }

        private Wallet()
        {
        }

        public Wallet(
            int id,
            int playerId,
            Currency currency,
            decimal balance,
            bool isBlocked = false)
        {
            if (balance < 0)
            {
                throw new InsufficientFundsException(balance);
            }

            Id = id;
            PlayerId = playerId;
            Currency = currency;
            Balance = balance;
            IsBlocked = isBlocked;
        }

        public void Block()
        {
            IsBlocked = true;
        }

        public void Unblock()
        {
            IsBlocked = false;
        }

        public void SetBalance(decimal balance)
        {
            if (balance < 0)
            {
                throw new InsufficientFundsException(balance);
            }

            Balance = balance;
        }

        public void Deposit(decimal amount)
        {
            ValidateAmount(amount);
            EnsureWalletIsNotBlocked();

            Balance += amount;
        }

        public void Withdraw(decimal amount)
        {
            ValidateAmount(amount);
            EnsureWalletIsNotBlocked();

            var newBalance = Balance - amount;

            if (newBalance < 0)
            {
                throw new InsufficientFundsException(newBalance);
            }

            Balance = newBalance;
        }

        public void ForceSubtractFunds(decimal amount)
        {
            ValidateAmount(amount);
            EnsureWalletIsNotBlocked();

            Balance -= amount;
        }

        private static void ValidateAmount(decimal amount)
        {
            if (amount <= 0)
            {
                throw new InvalidAmountException(amount);
            }
        }

        private void EnsureWalletIsNotBlocked()
        {
            if (IsBlocked)
            {
                throw new WalletBlockedException(Currency);
            }
        }

        public override string ToString()
        {
            return
                $"Id -> {Id} " +
                $"PlayerId -> {PlayerId} " +
                $"Balance -> {Balance} " +
                $"Currency -> {Currency} " +
                $"IsBlocked -> {IsBlocked}";
        }
    }
}