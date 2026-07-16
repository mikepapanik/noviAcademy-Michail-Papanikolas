using Moq;
using WorldRank.Application.Infrastructure.Players;
using WorldRank.Application.Queries.Players;
using WorldRank.Domain.Player;

namespace WorldRank.UnitTests.Handlers.Players;

public sealed class GetPlayerByIdQueryHandlerTests
{
    private readonly Mock<IGetPlayerByIdPersistence>
        _getPlayerByIdPersistence;

    private readonly GetPlayerByIdQueryHandler _handler;

    public GetPlayerByIdQueryHandlerTests()
    {
        _getPlayerByIdPersistence =
            new Mock<IGetPlayerByIdPersistence>();

        _handler = new GetPlayerByIdQueryHandler(
            _getPlayerByIdPersistence.Object);
    }

    [Fact]
    public async Task Handle_ExistingPlayer_ReturnsPlayer()
    {
        // Arrange
        var player = new Player(
            id: 1,
            name: "Michail");

        var query = new GetPlayerByIdQuery(
            Id: 1);

        _getPlayerByIdPersistence
            .Setup(persistence => persistence.GetByIdAsync(
                query.Id,
                It.IsAny<CancellationToken>()))
            .ReturnsAsync(player);

        // Act
        var result = await _handler.Handle(
            query,
            CancellationToken.None);

        // Assert
        Assert.Same(player, result);

        _getPlayerByIdPersistence.Verify(
            persistence => persistence.GetByIdAsync(
                query.Id,
                It.IsAny<CancellationToken>()),
            Times.Once);
    }

    [Fact]
    public async Task Handle_PlayerDoesNotExist_ReturnsNull()
    {
        // Arrange
        var query = new GetPlayerByIdQuery(
            Id: 999);

        _getPlayerByIdPersistence
            .Setup(persistence => persistence.GetByIdAsync(
                query.Id,
                It.IsAny<CancellationToken>()))
            .ReturnsAsync((Player?)null);

        // Act
        var result = await _handler.Handle(
            query,
            CancellationToken.None);

        // Assert
        Assert.Null(result);

        _getPlayerByIdPersistence.Verify(
            persistence => persistence.GetByIdAsync(
                query.Id,
                It.IsAny<CancellationToken>()),
            Times.Once);
    }
}
