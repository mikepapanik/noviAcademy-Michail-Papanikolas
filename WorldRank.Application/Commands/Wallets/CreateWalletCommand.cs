using MediatR;
using WorldRank.Domain.Enums;

namespace WorldRank.Application.Commands.Wallets;

public sealed record CreateWalletCommand(
    int Id,
    int PlayerId,
    Currency Currency,
    decimal Balance) : IRequest;
