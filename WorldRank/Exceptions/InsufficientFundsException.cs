namespace WorldRank.Exceptions;

public class InsufficientFundsException : WalletException
{
    public decimal Balance { get; }
    public decimal RequestedAmount { get; }

    public InsufficientFundsException(decimal balance, decimal requestedAmount)
        : base($"Insufficient funds. Balance: {balance}, Requested amount: {requestedAmount}.")
    {
        Balance = balance;
        RequestedAmount = requestedAmount;
    }
}