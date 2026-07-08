using System;

namespace WorldRank.Exceptions;

public class PlayerAlreadyExistsException : Exception
{
    public int PlayerId { get; }

    public PlayerAlreadyExistsException(int playerId)
        : base($"Player with id {playerId} already exists.")
    {
        PlayerId = playerId;
    }
}