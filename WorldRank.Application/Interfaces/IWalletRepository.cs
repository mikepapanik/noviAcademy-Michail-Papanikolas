using WorldRank.Domain.Enums;
using WorldRank.Domain.Wallets;

namespace WorldRank.Application.Interfaces;

public interface IWalletRepository
{
    Task AddAsync(
        Wallet wallet,
        CancellationToken cancellationToken);

    Task<Wallet?> GetByIdAsync(
        int walletId,
        CancellationToken cancellationToken);

    Task<Wallet?> GetWalletAsync(
        int playerId,
        Currency currency,
        CancellationToken cancellationToken);

    Task<IReadOnlyList<Wallet>> GetAllAsync(
        CancellationToken cancellationToken);

    Task<IReadOnlyList<Wallet>> GetAllWalletsByPlayerIdAsync(
        int playerId,
        CancellationToken cancellationToken);

    Task UpdateAsync(
        Wallet wallet,
        CancellationToken cancellationToken);
}