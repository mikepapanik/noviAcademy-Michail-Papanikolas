using MediatR;
using WorldRank.Domain.Wallets;

namespace WorldRank.Application.Queries.Wallets;

public sealed record GetWalletsByPlayerIdQuery(
    int PlayerId) : IRequest<IReadOnlyList<Wallet>>;
