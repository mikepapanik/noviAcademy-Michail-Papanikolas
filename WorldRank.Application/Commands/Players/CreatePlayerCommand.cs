using MediatR;

namespace WorldRank.Application.Commands.Players;

public sealed record CreatePlayerCommand(
    int Id,
    string Name,
    int Score) : IRequest;
