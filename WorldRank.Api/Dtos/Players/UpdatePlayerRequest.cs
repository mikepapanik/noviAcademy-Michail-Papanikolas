namespace WorldRank.Api.Dtos.Players;

public sealed record UpdatePlayerRequest(
    string Name,
    int Score);
