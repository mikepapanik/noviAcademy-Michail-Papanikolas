using WorldRank.Domain.Enums;

namespace WorldRank.Api.Dtos.Wallets;

public record WalletResponse(
    int Id,
    int PlayerId,
    Currency Currency,
    decimal Balance,
    bool IsBlocked);
