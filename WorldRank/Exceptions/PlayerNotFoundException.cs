using System;

namespace WorldRank.Exceptions;

public class PlayerNotFoundException : Exception
{
    public int PlayerId { get; }

    public PlayerNotFoundException(int playerId)
        : base($"Player with id {playerId} was not found.")
    {
        PlayerId = playerId;
    }
}