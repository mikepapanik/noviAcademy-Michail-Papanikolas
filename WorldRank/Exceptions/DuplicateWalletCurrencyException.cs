namespace WorldRank.Exceptions;

public class DuplicateWalletCurrencyException : WalletException
{
    public Currency Currency { get; }

    public DuplicateWalletCurrencyException(Currency currency)
        : base($"Player already has a wallet with currency {currency}.")
    {
        Currency = currency;
    }
}