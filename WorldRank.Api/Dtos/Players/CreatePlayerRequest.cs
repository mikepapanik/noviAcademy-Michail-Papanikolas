namespace WorldRank.Api.Dtos.Players;

public record CreatePlayerRequest(
    int Id,
    string Name,
    int Score);
