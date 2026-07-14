using MediatR;

namespace WorldRank.Application.Commands.Players;

public sealed record UpdatePlayerCommand(
    int Id,
    string Name,
    int Score) : IRequest<bool>;
