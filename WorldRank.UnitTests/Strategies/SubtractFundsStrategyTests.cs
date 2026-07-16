using WorldRank.Application.Strategies;
using WorldRank.Domain.Enums;
using WorldRank.Domain.Exceptions;
using WorldRank.Domain.Wallets;

namespace WorldRank.UnitTests.Strategies;

public sealed class SubtractFundsStrategyTests
{
    [Fact]
    public void Operation_ReturnsSubtract()
    {
        // Arrange
        var strategy = new SubtractFundsStrategy();

        // Act
        var operation = strategy.Operation;

        // Assert
        Assert.Equal(FundsOperation.Subtract, operation);
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

        var strategy = new SubtractFundsStrategy();

        // Act
        strategy.Execute(wallet, 40m);

        // Assert
        Assert.Equal(60m, wallet.Balance);
    }

    [Fact]
    public void Execute_AmountGreaterThanBalance_ThrowsInsufficientFundsException()
    {
        // Arrange
        var wallet = new Wallet(
            id: 1,
            playerId: 10,
            currency: Currency.EUR,
            balance: 100m);

        var strategy = new SubtractFundsStrategy();

        // Act
        var exception = Assert.Throws<InsufficientFundsException>(
            () => strategy.Execute(wallet, 150m));

        // Assert
        Assert.NotNull(exception);
        Assert.Equal(100m, wallet.Balance);
    }
}
