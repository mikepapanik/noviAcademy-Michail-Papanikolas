using WorldRank.Domain.Enums;

namespace WorldRank.Api.Dtos.Wallets;

public record CreateWalletRequest(
    int Id,
    int PlayerId,
    Currency Currency,
    decimal Balance);
