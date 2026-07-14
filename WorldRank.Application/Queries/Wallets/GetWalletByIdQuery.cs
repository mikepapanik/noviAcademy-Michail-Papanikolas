using MediatR;
using WorldRank.Domain.Wallets;

namespace WorldRank.Application.Queries.Wallets;

public sealed record GetWalletByIdQuery(
    int Id) : IRequest<Wallet?>;