namespace WorldRank.Application.Caching;

public static class CacheKeys
{
    public const string AllPlayers = "players:all";

    public static string PlayerById(int playerId)
    {
        return $"players:{playerId}";
    }

    public static string WalletById(int walletId)
    {
        return $"wallets:{walletId}";
    }

    public static string WalletsByPlayerId(int playerId)
    {
        return $"players:{playerId}:wallets";
    }
}
