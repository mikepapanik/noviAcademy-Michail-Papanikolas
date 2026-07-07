using System;
using System.Collections.Generic;
using System.Linq;

namespace WorldRank;

public class InMemoryPlayerRepository : IPlayerRepository
{
    private readonly Dictionary<int, Player> _players = new();

    public void AddPlayer(Player player)
    {
        if (!_players.TryAdd(player.Id, player))
            throw new InvalidOperationException("Player with this id already exists.");
    }

    public Player? FindPlayer(int playerId)
    {
        _players.TryGetValue(playerId, out Player? player);
        return player;
    }

    public bool DeletePlayer(int playerId)
    {
        return _players.Remove(playerId);
    }

    public IEnumerable<Player> GetAllPlayers()
    {
        return _players.Values.OrderByDescending(p => p.Score);
    }

    public IEnumerable<IGrouping<int, Player>> GroupPlayersByScore()
    {
        return _players.Values
            .GroupBy(p => p.Score)
            .OrderByDescending(g => g.Key);
    }
}