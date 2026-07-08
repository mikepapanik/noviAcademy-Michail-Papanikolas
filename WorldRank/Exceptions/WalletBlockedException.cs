namespace WorldRank.Exceptions;

public class WalletBlockedException : WalletException
{
    public int PlayerId { get; }

    public WalletBlockedException(int playerId)
        : base($"Wallet for player {playerId} is blocked.")
    {
        PlayerId = playerId;
    }
}