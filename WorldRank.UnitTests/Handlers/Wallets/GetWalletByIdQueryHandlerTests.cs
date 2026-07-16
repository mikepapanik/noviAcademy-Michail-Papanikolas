using Moq;
using WorldRank.Application.Infrastructure.Wallets;
using WorldRank.Application.Queries.Wallets;
using WorldRank.Domain.Enums;
using WorldRank.Domain.Wallets;

namespace WorldRank.UnitTests.Handlers.Wallets;

public sealed class GetWalletByIdQueryHandlerTests
{
    private readonly Mock<IGetWalletByIdPersistence>
        _getWalletByIdPersistence;

    private readonly GetWalletByIdQueryHandler _handler;

    public GetWalletByIdQueryHandlerTests()
    {
        _getWalletByIdPersistence =
            new Mock<IGetWalletByIdPersistence>();

        _handler = new GetWalletByIdQueryHandler(
            _getWalletByIdPersistence.Object);
    }

    [Fact]
    public async Task Handle_ExistingWallet_ReturnsWallet()
    {
        // Arrange
        var wallet = new Wallet(
            id: 1,
            playerId: 10,
            currency: Currency.EUR,
            balance: 100m);

        var query = new GetWalletByIdQuery(
            Id: wallet.Id);

        _getWalletByIdPersistence
            .Setup(persistence => persistence.GetByIdAsync(
                query.Id,
                It.IsAny<CancellationToken>()))
            .ReturnsAsync(wallet);

        // Act
        var result = await _handler.Handle(
            query,
            CancellationToken.None);

        // Assert
        Assert.Same(wallet, result);

        _getWalletByIdPersistence.Verify(
            persistence => persistence.GetByIdAsync(
                query.Id,
                It.IsAny<CancellationToken>()),
            Times.Once);
    }

    [Fact]
    public async Task Handle_WalletDoesNotExist_ReturnsNull()
    {
        // Arrange
        var query = new GetWalletByIdQuery(
            Id: 999);

        _getWalletByIdPersistence
            .Setup(persistence => persistence.GetByIdAsync(
                query.Id,
                It.IsAny<CancellationToken>()))
            .ReturnsAsync((Wallet?)null);

        // Act
        var result = await _handler.Handle(
            query,
            CancellationToken.None);

        // Assert
        Assert.Null(result);

        _getWalletByIdPersistence.Verify(
            persistence => persistence.GetByIdAsync(
                query.Id,
                It.IsAny<CancellationToken>()),
            Times.Once);
    }
}
