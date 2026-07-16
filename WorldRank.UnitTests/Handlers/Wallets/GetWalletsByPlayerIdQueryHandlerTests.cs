using Moq;
using WorldRank.Application.Infrastructure.Wallets;
using WorldRank.Application.Queries.Wallets;
using WorldRank.Domain.Enums;
using WorldRank.Domain.Wallets;

namespace WorldRank.UnitTests.Handlers.Wallets;

public sealed class GetWalletsByPlayerIdQueryHandlerTests
{
    private readonly Mock<IGetWalletsByPlayerIdPersistence>
        _getWalletsByPlayerIdPersistence;

    private readonly GetWalletsByPlayerIdQueryHandler _handler;

    public GetWalletsByPlayerIdQueryHandlerTests()
    {
        _getWalletsByPlayerIdPersistence =
            new Mock<IGetWalletsByPlayerIdPersistence>();

        _handler = new GetWalletsByPlayerIdQueryHandler(
            _getWalletsByPlayerIdPersistence.Object);
    }

    [Fact]
    public async Task Handle_PlayerHasWallets_ReturnsPlayerWallets()
    {
        // Arrange
        IReadOnlyList<Wallet> wallets =
        [
            new Wallet(
                id: 1,
                playerId: 10,
                currency: Currency.EUR,
                balance: 100m),

            new Wallet(
                id: 2,
                playerId: 10,
                currency: Currency.USD,
                balance: 200m)
        ];

        var query = new GetWalletsByPlayerIdQuery(
            PlayerId: 10);

        _getWalletsByPlayerIdPersistence
            .Setup(persistence => persistence.GetByPlayerIdAsync(
                query.PlayerId,
                It.IsAny<CancellationToken>()))
            .ReturnsAsync(wallets);

        // Act
        var result = await _handler.Handle(
            query,
            CancellationToken.None);

        // Assert
        Assert.Same(wallets, result);
        Assert.Equal(2, result.Count);
        Assert.All(
            result,
            wallet => Assert.Equal(
                query.PlayerId,
                wallet.PlayerId));

        _getWalletsByPlayerIdPersistence.Verify(
            persistence => persistence.GetByPlayerIdAsync(
                query.PlayerId,
                It.IsAny<CancellationToken>()),
            Times.Once);
    }

    [Fact]
    public async Task Handle_PlayerHasNoWallets_ReturnsEmptyCollection()
    {
        // Arrange
        IReadOnlyList<Wallet> wallets =
            Array.Empty<Wallet>();

        var query = new GetWalletsByPlayerIdQuery(
            PlayerId: 999);

        _getWalletsByPlayerIdPersistence
            .Setup(persistence => persistence.GetByPlayerIdAsync(
                query.PlayerId,
                It.IsAny<CancellationToken>()))
            .ReturnsAsync(wallets);

        // Act
        var result = await _handler.Handle(
            query,
            CancellationToken.None);

        // Assert
        Assert.NotNull(result);
        Assert.Empty(result);

        _getWalletsByPlayerIdPersistence.Verify(
            persistence => persistence.GetByPlayerIdAsync(
                query.PlayerId,
                It.IsAny<CancellationToken>()),
            Times.Once);
    }
}
