using System.Collections.Generic;
using System.Linq;

namespace WorldRank;

public interface IPlayerRepository
{
    void AddPlayer(Player player);
    Player? FindPlayer(int playerId);
    bool DeletePlayer(int playerId);
    IEnumerable<Player> GetAllPlayers();
    IEnumerable<IGrouping<int, Player>> GroupPlayersByScore();
}