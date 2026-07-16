using Moq;
using WorldRank.Application.Commands.Players;
using WorldRank.Application.Infrastructure.Players;

namespace WorldRank.UnitTests.Handlers.Players;

public sealed class DeletePlayerCommandHandlerTests
{
    private readonly Mock<IDeletePlayerPersistence>
        _deletePlayerPersistence;

    private readonly DeletePlayerCommandHandler _handler;

    public DeletePlayerCommandHandlerTests()
    {
        _deletePlayerPersistence =
            new Mock<IDeletePlayerPersistence>();

        _handler = new DeletePlayerCommandHandler(
            _deletePlayerPersistence.Object);
    }

    [Fact]
    public async Task Handle_ExistingPlayer_DeletesPlayerAndReturnsTrue()
    {
        // Arrange
        var command = new DeletePlayerCommand(
            Id: 1);

        _deletePlayerPersistence
            .Setup(persistence => persistence.DeleteAsync(
                command.Id,
                It.IsAny<CancellationToken>()))
            .ReturnsAsync(true);

        // Act
        var result = await _handler.Handle(
            command,
            CancellationToken.None);

        // Assert
        Assert.True(result);

        _deletePlayerPersistence.Verify(
            persistence => persistence.DeleteAsync(
                command.Id,
                It.IsAny<CancellationToken>()),
            Times.Once);
    }

    [Fact]
    public async Task Handle_PlayerDoesNotExist_ReturnsFalse()
    {
        // Arrange
        var command = new DeletePlayerCommand(
            Id: 999);

        _deletePlayerPersistence
            .Setup(persistence => persistence.DeleteAsync(
                command.Id,
                It.IsAny<CancellationToken>()))
            .ReturnsAsync(false);

        // Act
        var result = await _handler.Handle(
            command,
            CancellationToken.None);

        // Assert
        Assert.False(result);

        _deletePlayerPersistence.Verify(
            persistence => persistence.DeleteAsync(
                command.Id,
                It.IsAny<CancellationToken>()),
            Times.Once);
    }
}
