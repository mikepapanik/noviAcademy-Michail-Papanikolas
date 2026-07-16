using WorldRank.Domain.Player;

namespace WorldRank.UnitTests.Entities;

public sealed class PlayerTests
{
    [Fact]
    public void Constructor_WithValidValues_InitializesPropertiesCorrectly()
    {
        // Arrange
        const int id = 1;
        const string name = "Michail";

        // Act
        var player = new Player(id, name);

        // Assert
        Assert.Equal(id, player.Id);
        Assert.Equal(name, player.Name);
        Assert.Equal(0, player.Score);
    }

    [Theory]
    [InlineData("")]
    [InlineData(" ")]
    [InlineData("   ")]
    public void Constructor_WithInvalidName_ThrowsArgumentException(
        string invalidName)
    {
        // Act
        var exception = Assert.Throws<ArgumentException>(
            () => new Player(
                id: 1,
                name: invalidName));

        // Assert
        Assert.Equal("name", exception.ParamName);
        Assert.Contains(
            "Name cannot be empty",
            exception.Message);
    }

    [Fact]
    public void Constructor_WithNullName_ThrowsArgumentException()
    {
        // Act
        var exception = Assert.Throws<ArgumentException>(
            () => new Player(
                id: 1,
                name: null!));

        // Assert
        Assert.Equal("name", exception.ParamName);
    }

    [Fact]
    public void AddScore_WithPositivePoints_IncreasesScore()
    {
        // Arrange
        var player = CreatePlayer();

        // Act
        player.AddScore(50);

        // Assert
        Assert.Equal(50, player.Score);
    }

    [Fact]
    public void AddScore_CalledMultipleTimes_AccumulatesScore()
    {
        // Arrange
        var player = CreatePlayer();

        // Act
        player.AddScore(50);
        player.AddScore(30);

        // Assert
        Assert.Equal(80, player.Score);
    }

    [Fact]
    public void AddScore_WithZeroPoints_DoesNotChangeScore()
    {
        // Arrange
        var player = CreatePlayer();

        // Act
        player.AddScore(0);

        // Assert
        Assert.Equal(0, player.Score);
    }

    [Fact]
    public void AddScore_WithNegativePoints_ThrowsArgumentOutOfRangeException()
    {
        // Arrange
        var player = CreatePlayer();

        // Act
        var exception = Assert.Throws<ArgumentOutOfRangeException>(
            () => player.AddScore(-10));

        // Assert
        Assert.Equal("points", exception.ParamName);
        Assert.Equal(0, player.Score);
    }

    [Fact]
    public void ToString_ReturnsExpectedPlayerDescription()
    {
        // Arrange
        var player = new Player(
            id: 1,
            name: "Michail");

        player.AddScore(100);

        // Act
        var result = player.ToString();

        // Assert
        Assert.Equal(
            "[1] Michail - Score: 100",
            result);
    }

    private static Player CreatePlayer()
    {
        return new Player(
            id: 1,
            name: "Michail");
    }
}
