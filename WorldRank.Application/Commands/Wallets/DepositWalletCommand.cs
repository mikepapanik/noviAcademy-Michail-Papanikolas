using MediatR;
using WorldRank.Domain.Wallets;

namespace WorldRank.Application.Commands.Wallets;

public sealed record DepositWalletCommand(
    int WalletId,
    decimal Amount) : IRequest<Wallet>;
