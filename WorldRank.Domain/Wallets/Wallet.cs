using WorldRank.Domain.Enums;
using WorldRank.Domain.Exceptions;

namespace WorldRank.Domain.Wallets
{
    public class Wallet : IWallet
    {
        public int PlayerId { get; }
        public Currency Currency { get; }
        public decimal Balance { get; private set; }
        public bool IsBlocked { get; private set; }

        public Wallet(int playerId, Currency currency, decimal balance, bool isBlocked = false)
        {
            if (balance < 0)
                throw new InsufficientFundsException(balance);

            PlayerId = playerId;
            Currency = currency;
            Balance = balance;
            IsBlocked = isBlocked;
        }

        public void Block() => IsBlocked = true;

        public void Unblock() => IsBlocked = false;

        public void SetBalance(decimal balance)
        {
            if (balance < 0)
                throw new InsufficientFundsException(balance);

            Balance = balance;
        }

        public void Deposit(decimal amount)
        {
            if (amount <= 0)
                throw new InvalidAmountException(amount);

            if (IsBlocked)
                throw new WalletBlockedException(Currency);

            Balance += amount;
        }

        public void Withdraw(decimal amount)
        {
            if (amount <= 0)
                throw new InvalidAmountException(amount);

            if (IsBlocked)
                throw new WalletBlockedException(Currency);

            var newBalance = Balance - amount;

            if (newBalance < 0)
                throw new InsufficientFundsException(newBalance);

            Balance = newBalance;
        }

        public override string ToString()
        {
            return $"Balance -> {Balance} Currency -> {Currency} IsBlocked -> {IsBlocked}";
        }
    }
}