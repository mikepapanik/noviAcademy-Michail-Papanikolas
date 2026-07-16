using Moq;
using WorldRank.Application.Commands.CurrencyRates;
using WorldRank.Application.ExchangeRates;
using WorldRank.Application.Infrastructure.CurrencyRates;
using Xunit;
using CurrencyRatesAggregate =
    WorldRank.Domain.CurrencyRates.CurrencyRates;

namespace WorldRank.UnitTests.Handlers.CurrencyRates;

public sealed class UpsertCurrencyRatesCommandHandlerTests
{
    [Fact]
    public async Task Handle_WithRates_MapsAndPersistsCurrencyRates()
    {
        // Arrange
        var persistence =
            new Mock<IUpsertCurrencyRatesPersistence>();

        var cancellationToken =
            new CancellationTokenSource().Token;

        var referenceDate = new DateTime(
            2026,
            7,
            16,
            15,
            30,
            0,
            DateTimeKind.Utc);

        var request = new UpsertCurrencyRatesCommand(
            new[]
            {
                new CurrencyRateDto(
                    " usd ",
                    1.15m,
                    referenceDate),

                new CurrencyRateDto(
                    "GBP",
                    0.86m,
                    referenceDate)
            });

        IReadOnlyCollection<CurrencyRatesAggregate>?
            persistedRates = null;

        persistence
            .Setup(x => x.UpsertAsync(
                It.IsAny<
                    IReadOnlyCollection<
                        CurrencyRatesAggregate>>(),
                cancellationToken))
            .Callback<
                IReadOnlyCollection<CurrencyRatesAggregate>,
                CancellationToken>(
                (rates, _) => persistedRates = rates)
            .Returns(Task.CompletedTask);

        var handler =
            new UpsertCurrencyRatesCommandHandler(
                persistence.Object);

        // Act
        await handler.Handle(
            request,
            cancellationToken);

        // Assert
        Assert.NotNull(persistedRates);
        Assert.Equal(2, persistedRates.Count);

        var usd = Assert.Single(
            persistedRates,
            rate => rate.Currency == "USD");

        Assert.Equal(1.15m, usd.Rate);
        Assert.Equal(referenceDate.Date, usd.Date);

        var gbp = Assert.Single(
            persistedRates,
            rate => rate.Currency == "GBP");

        Assert.Equal(0.86m, gbp.Rate);
        Assert.Equal(referenceDate.Date, gbp.Date);

        persistence.Verify(
            x => x.UpsertAsync(
                It.IsAny<
                    IReadOnlyCollection<
                        CurrencyRatesAggregate>>(),
                cancellationToken),
            Times.Once);
    }

    [Fact]
    public async Task Handle_WithEmptyRates_DoesNotCallPersistence()
    {
        // Arrange
        var persistence =
            new Mock<IUpsertCurrencyRatesPersistence>();

        var request =
            new UpsertCurrencyRatesCommand(
                Array.Empty<CurrencyRateDto>());

        var handler =
            new UpsertCurrencyRatesCommandHandler(
                persistence.Object);

        // Act
        await handler.Handle(
            request,
            CancellationToken.None);

        // Assert
        persistence.Verify(
            x => x.UpsertAsync(
                It.IsAny<
                    IReadOnlyCollection<
                        CurrencyRatesAggregate>>(),
                It.IsAny<CancellationToken>()),
            Times.Never);
    }

    [Fact]
    public async Task Handle_WithInvalidRate_ThrowsArgumentOutOfRangeException()
    {
        // Arrange
        var persistence =
            new Mock<IUpsertCurrencyRatesPersistence>();

        var request = new UpsertCurrencyRatesCommand(
            new[]
            {
                new CurrencyRateDto(
                    "USD",
                    0m,
                    new DateTime(2026, 7, 16))
            });

        var handler =
            new UpsertCurrencyRatesCommandHandler(
                persistence.Object);

        // Act
        var action = () => handler.Handle(
            request,
            CancellationToken.None);

        // Assert
        await Assert.ThrowsAsync<
            ArgumentOutOfRangeException>(action);

        persistence.Verify(
            x => x.UpsertAsync(
                It.IsAny<
                    IReadOnlyCollection<
                        CurrencyRatesAggregate>>(),
                It.IsAny<CancellationToken>()),
            Times.Never);
    }
}
