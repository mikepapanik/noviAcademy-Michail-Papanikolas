using Microsoft.Extensions.Logging;
using Moq;
using WorldRank.Application.Caching;
using WorldRank.Application.Interfaces;
using WorldRank.Application.Services;
using WorldRank.Domain.Player;

namespace WorldRank.UnitTests.Services;

public class PlayerServiceTests
{
    private readonly Mock<IPlayerRepository> _playerRepositoryMock;
    private readonly Mock<ICache> _cacheMock;
    private readonly Mock<ILogger<PlayerService>> _loggerMock;
    private readonly PlayerService _sut;

    public PlayerServiceTests()
    {
        _playerRepositoryMock =
            new Mock<IPlayerRepository>();

        _cacheMock =
            new Mock<ICache>();

        _loggerMock =
            new Mock<ILogger<PlayerService>>();

        _sut = new PlayerService(
            _playerRepositoryMock.Object,
            _cacheMock.Object,
            _loggerMock.Object);
    }

    [Fact]
    public async Task AddPlayerAsync_ValidPlayer_AddsPlayerAndUpdatesCache()
    {
        // Arrange
        var player = CreatePlayer(
            id: 1,
            name: "Michail",
            score: 100);

        var cancellationToken =
            CancellationToken.None;

        // Act
        var result = await _sut.AddPlayerAsync(
            player,
            cancellationToken);

        // Assert
        Assert.Same(player, result);

        _playerRepositoryMock.Verify(
            repository => repository.AddPlayerAsync(
                player,
                cancellationToken),
            Times.Once);

        _cacheMock.Verify(
            cache => cache.SetAsync(
                "players:1",
                player,
                It.IsAny<TimeSpan>(),
                cancellationToken),
            Times.Once);

        _cacheMock.Verify(
            cache => cache.RemoveAsync(
                "players:all",
                cancellationToken),
            Times.Once);
    }

    [Fact]
    public async Task GetAllPlayersAsync_PlayersExistInCache_ReturnsCachedPlayers()
    {
        // Arrange
        IReadOnlyList<Player> cachedPlayers =
        [
            CreatePlayer(1, "Michail", 100),
            CreatePlayer(2, "Nikos", 80)
        ];

        var cancellationToken =
            CancellationToken.None;

        _cacheMock
            .Setup(cache => cache.GetAsync<IReadOnlyList<Player>>(
                "players:all",
                cancellationToken))
            .ReturnsAsync(cachedPlayers);

        // Act
        var result = await _sut.GetAllPlayersAsync(
            cancellationToken);

        // Assert
        Assert.Same(cachedPlayers, result);

        _playerRepositoryMock.Verify(
            repository => repository.GetAllPlayersAsync(
                It.IsAny<CancellationToken>()),
            Times.Never);

        _cacheMock.Verify(
            cache => cache.SetAsync(
                It.IsAny<string>(),
                It.IsAny<IReadOnlyList<Player>>(),
                It.IsAny<TimeSpan>(),
                It.IsAny<CancellationToken>()),
            Times.Never);
    }

    [Fact]
    public async Task GetAllPlayersAsync_CacheIsEmpty_LoadsPlayersFromRepositoryAndCachesThem()
    {
        // Arrange
        IReadOnlyList<Player> repositoryPlayers =
        [
            CreatePlayer(1, "Michail", 100),
            CreatePlayer(2, "Nikos", 80)
        ];

        var cancellationToken =
            CancellationToken.None;

        _cacheMock
            .Setup(cache => cache.GetAsync<IReadOnlyList<Player>>(
                "players:all",
                cancellationToken))
            .ReturnsAsync(
                (IReadOnlyList<Player>?)null);

        _playerRepositoryMock
            .Setup(repository => repository.GetAllPlayersAsync(
                cancellationToken))
            .ReturnsAsync(repositoryPlayers);

        // Act
        var result = await _sut.GetAllPlayersAsync(
            cancellationToken);

        // Assert
        Assert.Same(repositoryPlayers, result);

        _playerRepositoryMock.Verify(
            repository => repository.GetAllPlayersAsync(
                cancellationToken),
            Times.Once);

        _cacheMock.Verify(
            cache => cache.SetAsync(
                "players:all",
                repositoryPlayers,
                It.IsAny<TimeSpan>(),
                cancellationToken),
            Times.Once);
    }

    [Fact]
    public async Task FindPlayerAsync_PlayerExistsInCache_ReturnsCachedPlayer()
    {
        // Arrange
        var player = CreatePlayer(
            id: 1,
            name: "Michail",
            score: 100);

        var cancellationToken =
            CancellationToken.None;

        _cacheMock
            .Setup(cache => cache.GetAsync<Player>(
                "players:1",
                cancellationToken))
            .ReturnsAsync(player);

        // Act
        var result = await _sut.FindPlayerAsync(
            playerId: 1,
            cancellationToken);

        // Assert
        Assert.Same(player, result);

        _playerRepositoryMock.Verify(
            repository => repository.FindPlayerAsync(
                It.IsAny<int>(),
                It.IsAny<CancellationToken>()),
            Times.Never);
    }

    [Fact]
    public async Task FindPlayerAsync_PlayerNotInCacheButExistsInRepository_ReturnsAndCachesPlayer()
    {
        // Arrange
        var player = CreatePlayer(
            id: 1,
            name: "Michail",
            score: 100);

        var cancellationToken =
            CancellationToken.None;

        _cacheMock
            .Setup(cache => cache.GetAsync<Player>(
                "players:1",
                cancellationToken))
            .ReturnsAsync((Player?)null);

        _playerRepositoryMock
            .Setup(repository => repository.FindPlayerAsync(
                1,
                cancellationToken))
            .ReturnsAsync(player);

        // Act
        var result = await _sut.FindPlayerAsync(
            playerId: 1,
            cancellationToken);

        // Assert
        Assert.Same(player, result);

        _playerRepositoryMock.Verify(
            repository => repository.FindPlayerAsync(
                1,
                cancellationToken),
            Times.Once);

        _cacheMock.Verify(
            cache => cache.SetAsync(
                "players:1",
                player,
                It.IsAny<TimeSpan>(),
                cancellationToken),
            Times.Once);
    }

    [Fact]
    public async Task FindPlayerAsync_PlayerDoesNotExist_ReturnsNullAndDoesNotCache()
    {
        // Arrange
        var cancellationToken =
            CancellationToken.None;

        _cacheMock
            .Setup(cache => cache.GetAsync<Player>(
                "players:99",
                cancellationToken))
            .ReturnsAsync((Player?)null);

        _playerRepositoryMock
            .Setup(repository => repository.FindPlayerAsync(
                99,
                cancellationToken))
            .ReturnsAsync((Player?)null);

        // Act
        var result = await _sut.FindPlayerAsync(
            playerId: 99,
            cancellationToken);

        // Assert
        Assert.Null(result);

        _cacheMock.Verify(
            cache => cache.SetAsync(
                It.IsAny<string>(),
                It.IsAny<Player>(),
                It.IsAny<TimeSpan>(),
                It.IsAny<CancellationToken>()),
            Times.Never);
    }

    [Fact]
    public async Task DeletePlayerAsync_PlayerExists_DeletesPlayerAndInvalidatesCache()
    {
        // Arrange
        var player = CreatePlayer(
            id: 1,
            name: "Michail",
            score: 100);

        var cancellationToken =
            CancellationToken.None;

        _playerRepositoryMock
            .Setup(repository => repository.FindPlayerAsync(
                1,
                cancellationToken))
            .ReturnsAsync(player);

        // Act
        await _sut.DeletePlayerAsync(
            playerId: 1,
            cancellationToken);

        // Assert
        _playerRepositoryMock.Verify(
            repository => repository.DeletePlayerAsync(
                1,
                cancellationToken),
            Times.Once);

        _cacheMock.Verify(
            cache => cache.RemoveAsync(
                "players:all",
                cancellationToken),
            Times.Once);

        _cacheMock.Verify(
            cache => cache.RemoveAsync(
                "players:1",
                cancellationToken),
            Times.Once);
    }

    [Fact]
    public async Task DeletePlayerAsync_PlayerDoesNotExist_DoesNotDeleteOrInvalidateCache()
    {
        // Arrange
        var cancellationToken =
            CancellationToken.None;

        _playerRepositoryMock
            .Setup(repository => repository.FindPlayerAsync(
                99,
                cancellationToken))
            .ReturnsAsync((Player?)null);

        // Act
        await _sut.DeletePlayerAsync(
            playerId: 99,
            cancellationToken);

        // Assert
        _playerRepositoryMock.Verify(
            repository => repository.DeletePlayerAsync(
                It.IsAny<int>(),
                It.IsAny<CancellationToken>()),
            Times.Never);

        _cacheMock.Verify(
            cache => cache.RemoveAsync(
                It.IsAny<string>(),
                It.IsAny<CancellationToken>()),
            Times.Never);
    }

    [Fact]
    public async Task GroupPlayersByScoreAsync_PlayersWithDifferentScores_ReturnsGroupsInDescendingOrder()
    {
        // Arrange
        IReadOnlyList<Player> players =
        [
            CreatePlayer(1, "Michail", 100),
            CreatePlayer(2, "Nikos", 50),
            CreatePlayer(3, "Maria", 100)
        ];

        _cacheMock
            .Setup(cache => cache.GetAsync<IReadOnlyList<Player>>(
                "players:all",
                CancellationToken.None))
            .ReturnsAsync(players);

        // Act
        var result = await _sut.GroupPlayersByScoreAsync(
            CancellationToken.None);

        var groups = result.ToList();

        // Assert
        Assert.Equal(2, groups.Count);

        Assert.Equal(100, groups[0].Key);
        Assert.Equal(2, groups[0].Count());

        Assert.Equal(50, groups[1].Key);
        Assert.Single(groups[1]);
    }

    private static Player CreatePlayer(
        int id,
        string name,
        int score)
    {
        var player = new Player(
            id,
            name);

        player.AddScore(score);

        return player;
    }
}
