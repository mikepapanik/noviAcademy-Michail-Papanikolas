using MediatR;

namespace WorldRank.Application.Commands.Wallets;

public sealed record UnblockWalletCommand(
    int WalletId) : IRequest;
