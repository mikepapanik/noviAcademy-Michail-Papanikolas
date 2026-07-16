using WorldRank.Application.Strategies;
using WorldRank.Domain.Enums;
using WorldRank.Domain.Wallets;

namespace WorldRank.UnitTests.Strategies;

public sealed class ForceSubtractFundsStrategyTests
{
    [Fact]
    public void Operation_ReturnsForceSubtract()
    {
        // Arrange
        var strategy = new ForceSubtractFundsStrategy();

        // Act
        var operation = strategy.Operation;

        // Assert
        Assert.Equal(FundsOperation.ForceSubtract, operation);
    }

    [Fact]
    public void Execute_ValidAmount_DecreasesWalletBalance()
    {
        // Arrange
        var wallet = new Wallet(
            id: 1,
            playerId: 10,
            currency: Currency.EUR,
            balance: 100m);

        var strategy = new ForceSubtractFundsStrategy();

        // Act
        strategy.Execute(wallet, 40m);

        // Assert
        Assert.Equal(60m, wallet.Balance);
    }

    [Fact]
    public void Execute_AmountGreaterThanBalance_AllowsNegativeBalance()
    {
        // Arrange
        var wallet = new Wallet(
            id: 1,
            playerId: 10,
            currency: Currency.EUR,
            balance: 100m);

        var strategy = new ForceSubtractFundsStrategy();

        // Act
        strategy.Execute(wallet, 150m);

        // Assert
        Assert.Equal(-50m, wallet.Balance);
    }
}
