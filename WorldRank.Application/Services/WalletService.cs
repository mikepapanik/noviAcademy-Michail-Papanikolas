using NLog;
using WorldRank.Application.Interfaces;
using WorldRank.Application.Strategies;
using WorldRank.Domain.Enums;
using WorldRank.Domain.Exceptions;
using WorldRank.Domain.Wallets;

namespace WorldRank.Application.Services;

public class WalletService
{
    private readonly IWalletRepository _walletRepository;
    private readonly IPlayerRepository _playerRepository;
    private readonly IReadOnlyDictionary<FundsOperation, IFundsStrategy> _fundsStrategies;
    private readonly Logger _logger;

    public WalletService(
        IWalletRepository walletRepository,
        IPlayerRepository playerRepository,
        IEnumerable<IFundsStrategy> strategies)
    {
        _walletRepository = walletRepository;
        _playerRepository = playerRepository;
        _fundsStrategies = strategies.ToDictionary(strategy => strategy.Operation);
        _logger = LogManager.GetCurrentClassLogger();
    }

    public void AddWalletToPlayer(
        int id,
        int playerId,
        Currency currency,
        decimal balance)
    {
        if (_playerRepository.FindPlayer(playerId) is null)
            throw new PlayerNotFoundException(playerId);

        var wallet = new Wallet(id, playerId, currency, balance);
        _walletRepository.Add(wallet);

        _logger.Info(
            "Wallet {WalletId} created for player {PlayerId} in {Currency} with balance {Balance}",
            id,
            playerId,
            currency,
            balance);
    }

    public List<Wallet> GetAllWalletsByPlayerId(int playerId)
    {
        return _walletRepository.GetAllWalletsByPlayerId(playerId);
    }

    public void Deposit(int playerId, Currency currency, decimal amount)
    {
        ExecuteFundsOperation(
            playerId,
            currency,
            amount,
            FundsOperation.Add);
    }

    public void Withdraw(int playerId, Currency currency, decimal amount)
    {
        ExecuteFundsOperation(
            playerId,
            currency,
            amount,
            FundsOperation.Subtract);
    }

    public void ForceSubtract(int playerId, Currency currency, decimal amount)
    {
        ExecuteFundsOperation(
            playerId,
            currency,
            amount,
            FundsOperation.ForceSubtract);
    }

    public void UpdateBalance(
        int playerId,
        Currency currency,
        decimal newBalance)
    {
        _walletRepository.UpdateBalance(
            playerId,
            currency,
            newBalance);
    }

    public void Block(int playerId, Currency currency)
    {
        _walletRepository.Block(playerId, currency);
    }

    public void Unblock(int playerId, Currency currency)
    {
        _walletRepository.Unblock(playerId, currency);
    }

    private void ExecuteFundsOperation(
        int playerId,
        Currency currency,
        decimal amount,
        FundsOperation operation)
    {
        var wallet = _walletRepository.GetWallet(playerId, currency);

        if (!_fundsStrategies.TryGetValue(operation, out var strategy))
        {
            throw new InvalidOperationException(
                $"No funds strategy registered for operation {operation}.");
        }

        strategy.Execute(wallet, amount);

        _logger.Info(
            "Funds operation {Operation} executed for player {PlayerId} in {Currency} with amount {Amount}",
            operation,
            playerId,
            currency,
            amount);
    }
}