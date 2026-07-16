using WorldRank.Application.Strategies;
using WorldRank.Domain.Enums;
using WorldRank.Domain.Wallets;

namespace WorldRank.UnitTests.Strategies;

public sealed class AddFundsStrategyTests
{
    [Fact]
    public void Operation_ReturnsAdd()
    {
        // Arrange
        var strategy = new AddFundsStrategy();

        // Act
        var operation = strategy.Operation;

        // Assert
        Assert.Equal(FundsOperation.Add, operation);
    }

    [Fact]
    public void Execute_ValidAmount_IncreasesWalletBalance()
    {
        // Arrange
        var wallet = new Wallet(
            id: 1,
            playerId: 10,
            currency: Currency.EUR,
            balance: 100m);

        var strategy = new AddFundsStrategy();

        // Act
        strategy.Execute(wallet, 50m);

        // Assert
        Assert.Equal(150m, wallet.Balance);
    }
}
