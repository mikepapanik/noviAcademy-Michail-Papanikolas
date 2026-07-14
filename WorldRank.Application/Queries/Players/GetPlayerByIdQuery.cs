using MediatR;
using WorldRank.Domain.Player;

namespace WorldRank.Application.Queries.Players;

public sealed record GetPlayerByIdQuery(
    int Id) : IRequest<Player?>;
