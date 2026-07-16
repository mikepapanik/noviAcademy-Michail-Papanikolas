using Moq;
using WorldRank.Application.Commands.Players;
using WorldRank.Application.Infrastructure.Players;
using WorldRank.Domain.Player;

namespace WorldRank.UnitTests.Handlers.Players;

public sealed class UpdatePlayerCommandHandlerTests
{
    private readonly Mock<IUpdatePlayerPersistence>
        _updatePlayerPersistence;

    private readonly UpdatePlayerCommandHandler _handler;

    public UpdatePlayerCommandHandlerTests()
    {
        _updatePlayerPersistence =
            new Mock<IUpdatePlayerPersistence>();

        _handler = new UpdatePlayerCommandHandler(
            _updatePlayerPersistence.Object);
    }

    [Fact]
    public async Task Handle_ValidCommand_UpdatesPlayerAndReturnsTrue()
    {
        // Arrange
        var command = new UpdatePlayerCommand(
            Id: 1,
            Name: "Updated Michail",
            Score: 250);

        _updatePlayerPersistence
            .Setup(persistence => persistence.UpdateAsync(
                It.IsAny<Player>(),
                It.IsAny<CancellationToken>()))
            .ReturnsAsync(true);

        // Act
        var result = await _handler.Handle(
            command,
            CancellationToken.None);

        // Assert
        Assert.True(result);

        _updatePlayerPersistence.Verify(
            persistence => persistence.UpdateAsync(
                It.Is<Player>(player =>
                    player.Id == command.Id &&
                    player.Name == command.Name &&
                    player.Score == command.Score),
                It.IsAny<CancellationToken>()),
            Times.Once);
    }

    [Fact]
    public async Task Handle_PersistenceCannotUpdate_ReturnsFalse()
    {
        // Arrange
        var command = new UpdatePlayerCommand(
            Id: 999,
            Name: "Missing Player",
            Score: 100);

        _updatePlayerPersistence
            .Setup(persistence => persistence.UpdateAsync(
                It.IsAny<Player>(),
                It.IsAny<CancellationToken>()))
            .ReturnsAsync(false);

        // Act
        var result = await _handler.Handle(
            command,
            CancellationToken.None);

        // Assert
        Assert.False(result);

        _updatePlayerPersistence.Verify(
            persistence => persistence.UpdateAsync(
                It.Is<Player>(player =>
                    player.Id == command.Id &&
                    player.Name == command.Name &&
                    player.Score == command.Score),
                It.IsAny<CancellationToken>()),
            Times.Once);
    }

    [Fact]
    public async Task Handle_NegativeScore_ThrowsArgumentOutOfRangeException()
    {
        // Arrange
        var command = new UpdatePlayerCommand(
            Id: 1,
            Name: "Michail",
            Score: -10);

        // Act
        var action = async () =>
            await _handler.Handle(
                command,
                CancellationToken.None);

        // Assert
        await Assert.ThrowsAsync<ArgumentOutOfRangeException>(
            action);

        _updatePlayerPersistence.Verify(
            persistence => persistence.UpdateAsync(
                It.IsAny<Player>(),
                It.IsAny<CancellationToken>()),
            Times.Never);
    }
}
