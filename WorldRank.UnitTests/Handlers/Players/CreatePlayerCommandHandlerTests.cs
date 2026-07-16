using Moq;
using WorldRank.Application.Commands.Players;
using WorldRank.Application.Infrastructure.Players;
using WorldRank.Domain.Player;

namespace WorldRank.UnitTests.Handlers.Players;

public sealed class CreatePlayerCommandHandlerTests
{
    private readonly Mock<ICreatePlayerPersistence>
        _createPlayerPersistence;

    private readonly CreatePlayerCommandHandler _handler;

    public CreatePlayerCommandHandlerTests()
    {
        _createPlayerPersistence =
            new Mock<ICreatePlayerPersistence>();

        _handler = new CreatePlayerCommandHandler(
            _createPlayerPersistence.Object);
    }

    [Fact]
    public async Task Handle_ValidCommand_CreatesPlayerWithCorrectProperties()
    {
        // Arrange
        var command = new CreatePlayerCommand(
            Id: 1,
            Name: "Michail",
            Score: 100);

        // Act
        await _handler.Handle(
            command,
            CancellationToken.None);

        // Assert
        _createPlayerPersistence.Verify(
            persistence => persistence.CreateAsync(
                It.Is<Player>(player =>
                    player.Id == command.Id &&
                    player.Name == command.Name &&
                    player.Score == command.Score),
                It.IsAny<CancellationToken>()),
            Times.Once);
    }

    [Fact]
    public async Task Handle_EmptyName_ThrowsArgumentException()
    {
        // Arrange
        var command = new CreatePlayerCommand(
            Id: 1,
            Name: string.Empty,
            Score: 100);

        // Act
        var action = async () =>
            await _handler.Handle(
                command,
                CancellationToken.None);

        // Assert
        await Assert.ThrowsAsync<ArgumentException>(
            action);

        _createPlayerPersistence.Verify(
            persistence => persistence.CreateAsync(
                It.IsAny<Player>(),
                It.IsAny<CancellationToken>()),
            Times.Never);
    }

    [Fact]
    public async Task Handle_NegativeScore_ThrowsArgumentOutOfRangeException()
    {
        // Arrange
        var command = new CreatePlayerCommand(
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

        _createPlayerPersistence.Verify(
            persistence => persistence.CreateAsync(
                It.IsAny<Player>(),
                It.IsAny<CancellationToken>()),
            Times.Never);
    }
}
