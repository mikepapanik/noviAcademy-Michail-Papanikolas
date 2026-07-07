using System;

namespace WorldRank;

public class Wallet
{
    public int PlayerId { get; }
    public decimal Balance { get; private set; }
    public Currency Currency { get; }
    public bool IsBlocked { get; private set; }

    public Wallet(int playerId, Currency currency)
    {
        if (playerId <= 0)
            throw new ArgumentException("Player id must be positive.", nameof(playerId));

        PlayerId = playerId;
        Currency = currency;
        Balance = 0;
        IsBlocked = false;
    }

    public void Deposit(decimal amount)
    {
        if (IsBlocked)
            throw new InvalidOperationException("Wallet is blocked.");

        if (amount <= 0)
            throw new ArgumentOutOfRangeException(nameof(amount), "Amount must be positive.");

        Balance += amount;
    }

    public void Withdraw(decimal amount)
    {
        if (IsBlocked)
            throw new InvalidOperationException("Wallet is blocked.");

        if (amount <= 0)
            throw new ArgumentOutOfRangeException(nameof(amount), "Amount must be positive.");

        if (Balance - amount < 0)
            throw new InvalidOperationException("Balance cannot be negative.");

        Balance -= amount;
    }

    public void Block()
    {
        IsBlocked = true;
    }

    public override string ToString()
    {
        return $"PlayerId: {PlayerId} | Balance: {Balance} {Currency} | Blocked: {IsBlocked}";
    }
}