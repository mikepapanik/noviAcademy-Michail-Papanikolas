using Moq;
using WorldRank.Application.Commands.Wallets;
using WorldRank.Application.Infrastructure.Wallets;
using WorldRank.Domain.Enums;
using WorldRank.Domain.Exceptions;
using WorldRank.Domain.Wallets;

namespace WorldRank.UnitTests.Handlers.Wallets;

public sealed class UnblockWalletCommandHandlerTests
{
    private readonly Mock<IGetWalletByIdPersistence>
        _getWalletByIdPersistence;

    private readonly Mock<IUpdateWalletPersistence>
        _updateWalletPersistence;

    private readonly UnblockWalletCommandHandler _handler;

    public UnblockWalletCommandHandlerTests()
    {
        _getWalletByIdPersistence =
            new Mock<IGetWalletByIdPersistence>();

        _updateWalletPersistence =
            new Mock<IUpdateWalletPersistence>();

        _handler = new UnblockWalletCommandHandler(
            _getWalletByIdPersistence.Object,
            _updateWalletPersistence.Object);
    }

    [Fact]
    public async Task Handle_BlockedWallet_UnblocksAndUpdatesWallet()
    {
        // Arrange
        var wallet = new Wallet(
            id: 1,
            playerId: 10,
            currency: Currency.EUR,
            balance: 100m,
            isBlocked: true);

        var command = new UnblockWalletCommand(
            WalletId: wallet.Id);

        _getWalletByIdPersistence
            .Setup(persistence => persistence.GetByIdAsync(
                command.WalletId,
                It.IsAny<CancellationToken>()))
            .ReturnsAsync(wallet);

        // Act
        await _handler.Handle(
            command,
            CancellationToken.None);

        // Assert
        Assert.False(wallet.IsBlocked);

        _updateWalletPersistence.Verify(
            persistence => persistence.UpdateAsync(
                It.Is<Wallet>(updatedWallet =>
                    updatedWallet.Id == wallet.Id &&
                    updatedWallet.IsBlocked == false),
                It.IsAny<CancellationToken>()),
            Times.Once);
    }

    [Fact]
    public async Task Handle_WalletDoesNotExist_ThrowsWalletNotFoundException()
    {
        // Arrange
        var command = new UnblockWalletCommand(
            WalletId: 999);

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
}
