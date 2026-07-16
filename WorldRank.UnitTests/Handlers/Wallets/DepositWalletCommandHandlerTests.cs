using Moq;
using WorldRank.Application.Commands.Wallets;
using WorldRank.Application.Infrastructure.Wallets;
using WorldRank.Domain.Enums;
using WorldRank.Domain.Exceptions;
using WorldRank.Domain.Wallets;

namespace WorldRank.UnitTests.Handlers.Wallets;

public sealed class DepositWalletCommandHandlerTests
{
    private readonly Mock<IGetWalletByIdPersistence>
        _getWalletByIdPersistence;

    private readonly Mock<IUpdateWalletPersistence>
        _updateWalletPersistence;

    private readonly DepositWalletCommandHandler _handler;

    public DepositWalletCommandHandlerTests()
    {
        _getWalletByIdPersistence =
            new Mock<IGetWalletByIdPersistence>();

        _updateWalletPersistence =
            new Mock<IUpdateWalletPersistence>();

        _handler = new DepositWalletCommandHandler(
            _getWalletByIdPersistence.Object,
            _updateWalletPersistence.Object);
    }

    [Fact]
    public async Task Handle_ExistingWallet_DepositsAmountAndUpdatesWallet()
    {
        // Arrange
        var wallet = CreateWallet();

        var command = new DepositWalletCommand(
            WalletId: wallet.Id,
            Amount: 50m);

        _getWalletByIdPersistence
            .Setup(persistence => persistence.GetByIdAsync(
                command.WalletId,
                It.IsAny<CancellationToken>()))
            .ReturnsAsync(wallet);

        // Act
        var result = await _handler.Handle(
            command,
            CancellationToken.None);

        // Assert
        Assert.Same(wallet, result);
        Assert.Equal(150m, result.Balance);

        _updateWalletPersistence.Verify(
            persistence => persistence.UpdateAsync(
                wallet,
                It.IsAny<CancellationToken>()),
            Times.Once);
    }

    [Fact]
    public async Task Handle_WalletDoesNotExist_ThrowsWalletNotFoundException()
    {
        // Arrange
        var command = new DepositWalletCommand(
            WalletId: 999,
            Amount: 50m);

        _getWalletByIdPersistence
            .Setup(persistence => persistence.GetByIdAsync(
                command.WalletId,
                It.IsAny<CancellationToken>()))
            .ReturnsAsync((Wallet?)null);

        // Act
        var action = async () =>
            await _handler.Handle(
                command,
                CancellationToken.None);

        // Assert
        await Assert.ThrowsAsync<WalletNotFoundException>(
            action);

        _updateWalletPersistence.Verify(
            persistence => persistence.UpdateAsync(
                It.IsAny<Wallet>(),
                It.IsAny<CancellationToken>()),
            Times.Never);
    }

    [Fact]
    public async Task Handle_InvalidAmount_ThrowsInvalidAmountExceptionWithoutUpdating()
    {
        // Arrange
        var wallet = CreateWallet();

        var command = new DepositWalletCommand(
            WalletId: wallet.Id,
            Amount: 0m);

        _getWalletByIdPersistence
            .Setup(persistence => persistence.GetByIdAsync(
                command.WalletId,
                It.IsAny<CancellationToken>()))
            .ReturnsAsync(wallet);

        // Act
        var action = async () =>
            await _handler.Handle(
                command,
                CancellationToken.None);

        // Assert
        await Assert.ThrowsAsync<InvalidAmountException>(
            action);

        Assert.Equal(100m, wallet.Balance);

        _updateWalletPersistence.Verify(
            persistence => persistence.UpdateAsync(
                It.IsAny<Wallet>(),
                It.IsAny<CancellationToken>()),
            Times.Never);
    }

    private static Wallet CreateWallet()
    {
        return new Wallet(
            id: 1,
            playerId: 10,
            currency: Currency.EUR,
            balance: 100m);
    }
}
