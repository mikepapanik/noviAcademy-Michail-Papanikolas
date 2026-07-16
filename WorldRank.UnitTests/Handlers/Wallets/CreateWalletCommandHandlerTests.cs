using Moq;
using WorldRank.Application.Commands.Wallets;
using WorldRank.Application.Infrastructure.Players;
using WorldRank.Application.Infrastructure.Wallets;
using WorldRank.Domain.Enums;
using WorldRank.Domain.Exceptions;
using WorldRank.Domain.Player;
using WorldRank.Domain.Wallets;

namespace WorldRank.UnitTests.Handlers.Wallets;

public sealed class CreateWalletCommandHandlerTests
{
    private readonly Mock<IGetPlayerByIdPersistence>
        _getPlayerByIdPersistence;

    private readonly Mock<ICreateWalletPersistence>
        _createWalletPersistence;

    private readonly CreateWalletCommandHandler _handler;

    public CreateWalletCommandHandlerTests()
    {
        _getPlayerByIdPersistence =
            new Mock<IGetPlayerByIdPersistence>();

        _createWalletPersistence =
            new Mock<ICreateWalletPersistence>();

        _handler = new CreateWalletCommandHandler(
            _getPlayerByIdPersistence.Object,
            _createWalletPersistence.Object);
    }

    [Fact]
    public async Task Handle_ExistingPlayer_CreatesWalletWithCorrectProperties()
    {
        // Arrange
        var player = new Player(
            id: 10,
            name: "Michail");

        var command = new CreateWalletCommand(
            Id: 1,
            PlayerId: player.Id,
            Currency: Currency.EUR,
            Balance: 100m);

        _getPlayerByIdPersistence
            .Setup(persistence => persistence.GetByIdAsync(
                command.PlayerId,
                It.IsAny<CancellationToken>()))
            .ReturnsAsync(player);

        // Act
        await _handler.Handle(
            command,
            CancellationToken.None);

        // Assert
        _createWalletPersistence.Verify(
            persistence => persistence.CreateAsync(
                It.Is<Wallet>(wallet =>
                    wallet.Id == command.Id &&
                    wallet.PlayerId == command.PlayerId &&
                    wallet.Currency == command.Currency &&
                    wallet.Balance == command.Balance &&
                    wallet.IsBlocked == false),
                It.IsAny<CancellationToken>()),
            Times.Once);
    }

    [Fact]
    public async Task Handle_PlayerDoesNotExist_ThrowsPlayerNotFoundException()
    {
        // Arrange
        var command = new CreateWalletCommand(
            Id: 1,
            PlayerId: 999,
            Currency: Currency.EUR,
            Balance: 100m);

        _getPlayerByIdPersistence
            .Setup(persistence => persistence.GetByIdAsync(
                command.PlayerId,
                It.IsAny<CancellationToken>()))
            .ReturnsAsync((Player?)null);

        // Act
        var action = async () =>
            await _handler.Handle(
                command,
                CancellationToken.None);

        // Assert
        await Assert.ThrowsAsync<PlayerNotFoundException>(
            action);

        _createWalletPersistence.Verify(
            persistence => persistence.CreateAsync(
                It.IsAny<Wallet>(),
                It.IsAny<CancellationToken>()),
            Times.Never);
    }

    [Fact]
    public async Task Handle_NegativeBalance_ThrowsInsufficientFundsException()
    {
        // Arrange
        var player = new Player(
            id: 10,
            name: "Michail");

        var command = new CreateWalletCommand(
            Id: 1,
            PlayerId: player.Id,
            Currency: Currency.EUR,
            Balance: -100m);

        _getPlayerByIdPersistence
            .Setup(persistence => persistence.GetByIdAsync(
                command.PlayerId,
                It.IsAny<CancellationToken>()))
            .ReturnsAsync(player);

        // Act
        var action = async () =>
            await _handler.Handle(
                command,
                CancellationToken.None);

        // Assert
        await Assert.ThrowsAsync<InsufficientFundsException>(
            action);

        _createWalletPersistence.Verify(
            persistence => persistence.CreateAsync(
                It.IsAny<Wallet>(),
                It.IsAny<CancellationToken>()),
            Times.Never);
    }
}
