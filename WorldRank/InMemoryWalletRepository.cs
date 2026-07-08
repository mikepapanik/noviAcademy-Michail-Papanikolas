using System;
using System.Collections.Generic;
using System.Linq;

namespace WorldRank;

public class InMemoryWalletRepository : IWalletRepository
{
    private readonly Dictionary<int, List<Wallet>> _walletsByPlayer = new();

    public void Add(Wallet wallet, int playerId)
    {
        if (wallet.PlayerId != playerId)
            throw new InvalidOperationException("Wallet does not belong to this player.");

        if (!_walletsByPlayer.ContainsKey(playerId))
        {
            _walletsByPlayer[playerId] = new List<Wallet>();
        }

        if (_walletsByPlayer[playerId].Any(w => w.Currency == wallet.Currency))
            throw new InvalidOperationException("Player already has a wallet with this currency.");

        _walletsByPlayer[playerId].Add(wallet);
    }

    public IEnumerable<Wallet> GetByPlayer(int playerId)
    {
        if (!_walletsByPlayer.TryGetValue(playerId, out List<Wallet>? wallets))
        {
            return Enumerable.Empty<Wallet>();
        }

        return wallets;
    }
}