using MediatR;

namespace WorldRank.Application.Commands.Wallets;

public sealed record BlockWalletCommand(
    int WalletId) : IRequest;
