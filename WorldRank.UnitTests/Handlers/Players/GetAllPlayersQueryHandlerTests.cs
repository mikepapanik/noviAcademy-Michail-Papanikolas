using Moq;
using WorldRank.Application.Infrastructure.Players;
using WorldRank.Application.Queries.Players;
using WorldRank.Domain.Player;

namespace WorldRank.UnitTests.Handlers.Players;

public sealed class GetAllPlayersQueryHandlerTests
{
    private readonly Mock<IGetAllPlayersPersistence>
        _getAllPlayersPersistence;

    private readonly GetAllPlayersQueryHandler _handler;

    public GetAllPlayersQueryHandlerTests()
    {
        _getAllPlayersPersistence =
            new Mock<IGetAllPlayersPersistence>();

        _handler = new GetAllPlayersQueryHandler(
            _getAllPlayersPersistence.Object);
    }

    [Fact]
    public async Task Handle_PlayersExist_ReturnsAllPlayers()
    {
        // Arrange
        IReadOnlyList<Player> players =
        [
            new Player(1, "Michail"),
            new Player(2, "Nikos")
        ];

        var query = new GetAllPlayersQuery();

        _getAllPlayersPersistence
            .Setup(persistence => persistence.GetAllAsync(
                It.IsAny<CancellationToken>()))
            .ReturnsAsync(players);

        // Act
        var result = await _handler.Handle(
            query,
            CancellationToken.None);

        // Assert
        Assert.Same(players, result);
        Assert.Equal(2, result.Count);

        _getAllPlayersPersistence.Verify(
            persistence => persistence.GetAllAsync(
                It.IsAny<CancellationToken>()),
            Times.Once);
    }

    [Fact]
    public async Task Handle_NoPlayersExist_ReturnsEmptyCollection()
    {
        // Arrange
        IReadOnlyList<Player> players =
            Array.Empty<Player>();

        var query = new GetAllPlayersQuery();

        _getAllPlayersPersistence
            .Setup(persistence => persistence.GetAllAsync(
                It.IsAny<CancellationToken>()))
            .ReturnsAsync(players);

        // Act
        var result = await _handler.Handle(
            query,
            CancellationToken.None);

        // Assert
        Assert.NotNull(result);
        Assert.Empty(result);

        _getAllPlayersPersistence.Verify(
            persistence => persistence.GetAllAsync(
                It.IsAny<CancellationToken>()),
            Times.Once);
    }
}
