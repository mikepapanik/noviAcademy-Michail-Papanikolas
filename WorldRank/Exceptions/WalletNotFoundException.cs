namespace WorldRank.Exceptions;

public class WalletNotFoundException : WalletException
{
    public int PlayerId { get; }

    public WalletNotFoundException(int playerId)
        : base($"No wallet found for player {playerId}.")
    {
        PlayerId = playerId;
    }
}