using MediatR;

namespace WorldRank.Application.Commands.Players;

public sealed record DeletePlayerCommand(
    int Id) : IRequest<bool>;
