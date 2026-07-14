using MediatR;

namespace WorldRank.Application.Commands.Wallets;

public sealed record WithdrawWalletCommand(
    int WalletId,
    decimal Amount) : IRequest;
