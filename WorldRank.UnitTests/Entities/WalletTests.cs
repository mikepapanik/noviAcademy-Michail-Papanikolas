using WorldRank.Domain.Enums;
using WorldRank.Domain.Exceptions;
using WorldRank.Domain.Wallets;

namespace WorldRank.UnitTests.Entities;

public sealed class WalletTests
{
    [Fact]
    public void Constructor_WithValidValues_CreatesWallet()
    {
        // Arrange
        const int walletId = 1;
        const int playerId = 10;
        const decimal balance = 100m;

        // Act
        var wallet = new Wallet(
            walletId,
            playerId,
            Currency.EUR,
            balance);

        // Assert
        Assert.Equal(walletId, wallet.Id);
        Assert.Equal(playerId, wallet.PlayerId);
        Assert.Equal(Currency.EUR, wallet.Currency);
        Assert.Equal(balance, wallet.Balance);
        Assert.False(wallet.IsBlocked);
    }

    [Fact]
    public void Constructor_WithNegativeBalance_ThrowsInsufficientFundsException()
    {
        // Act
        var exception = Assert.Throws<InsufficientFundsException>(
            () => new Wallet(
                id: 1,
                playerId: 10,
                currency: Currency.EUR,
                balance: -1m));

        // Assert
        Assert.Equal(-1m, exception.AttemptedBalance);
    }

    [Fact]
    public void Deposit_WithValidAmount_IncreasesBalance()
    {
        // Arrange
        var wallet = CreateWallet(balance: 100m);

        // Act
        wallet.Deposit(50m);

        // Assert
        Assert.Equal(150m, wallet.Balance);
    }

    [Theory]
    [InlineData(0)]
    [InlineData(-10)]
    public void Deposit_WithInvalidAmount_ThrowsInvalidAmountException(
        int amount)
    {
        // Arrange
        var wallet = CreateWallet(balance: 100m);

        // Act
        var exception = Assert.Throws<InvalidAmountException>(
            () => wallet.Deposit(amount));

        // Assert
        Assert.Equal(amount, exception.Amount);
        Assert.Equal(100m, wallet.Balance);
    }

    [Fact]
    public void Deposit_WhenWalletIsBlocked_ThrowsWalletBlockedException()
    {
        // Arrange
        var wallet = CreateWallet(
            balance: 100m,
            isBlocked: true);

        // Act
        var exception = Assert.Throws<WalletBlockedException>(
            () => wallet.Deposit(50m));

        // Assert
        Assert.Equal(Currency.EUR, exception.Currency);
        Assert.Equal(100m, wallet.Balance);
    }

    [Fact]
    public void Withdraw_WithValidAmount_DecreasesBalance()
    {
        // Arrange
        var wallet = CreateWallet(balance: 100m);

        // Act
        wallet.Withdraw(40m);

        // Assert
        Assert.Equal(60m, wallet.Balance);
    }

    [Fact]
    public void Withdraw_WithInsufficientBalance_ThrowsInsufficientFundsException()
    {
        // Arrange
        var wallet = CreateWallet(balance: 50m);

        // Act
        var exception = Assert.Throws<InsufficientFundsException>(
            () => wallet.Withdraw(80m));

        // Assert
        Assert.Equal(-30m, exception.AttemptedBalance);
        Assert.Equal(50m, wallet.Balance);
    }

    [Fact]
    public void Withdraw_WhenWalletIsBlocked_ThrowsWalletBlockedException()
    {
        // Arrange
        var wallet = CreateWallet(
            balance: 100m,
            isBlocked: true);

        // Act
        var exception = Assert.Throws<WalletBlockedException>(
            () => wallet.Withdraw(20m));

        // Assert
        Assert.Equal(Currency.EUR, exception.Currency);
        Assert.Equal(100m, wallet.Balance);
    }

    [Fact]
    public void Block_SetsWalletAsBlocked()
    {
        // Arrange
        var wallet = CreateWallet();

        // Act
        wallet.Block();

        // Assert
        Assert.True(wallet.IsBlocked);
    }

    [Fact]
    public void Unblock_SetsWalletAsNotBlocked()
    {
        // Arrange
        var wallet = CreateWallet(isBlocked: true);

        // Act
        wallet.Unblock();

        // Assert
        Assert.False(wallet.IsBlocked);
    }

    [Fact]
    public void SetBalance_WithValidValue_UpdatesBalance()
    {
        // Arrange
        var wallet = CreateWallet(balance: 100m);

        // Act
        wallet.SetBalance(250m);

        // Assert
        Assert.Equal(250m, wallet.Balance);
    }

    [Fact]
    public void SetBalance_WithNegativeValue_ThrowsInsufficientFundsException()
    {
        // Arrange
        var wallet = CreateWallet(balance: 100m);

        // Act
        var exception = Assert.Throws<InsufficientFundsException>(
            () => wallet.SetBalance(-50m));

        // Assert
        Assert.Equal(-50m, exception.AttemptedBalance);
        Assert.Equal(100m, wallet.Balance);
    }

    [Fact]
    public void ForceSubtractFunds_WithValidAmount_SubtractsAmount()
    {
        // Arrange
        var wallet = CreateWallet(balance: 100m);

        // Act
        wallet.ForceSubtractFunds(150m);

        // Assert
        Assert.Equal(-50m, wallet.Balance);
    }

    private static Wallet CreateWallet(
        decimal balance = 100m,
        bool isBlocked = false)
    {
        return new Wallet(
            id: 1,
            playerId: 10,
            currency: Currency.EUR,
            balance: balance,
            isBlocked: isBlocked);
    }
}