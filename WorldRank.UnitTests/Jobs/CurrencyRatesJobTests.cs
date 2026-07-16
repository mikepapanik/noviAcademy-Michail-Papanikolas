using MediatR;
using Microsoft.Extensions.Logging;
using Moq;
using Quartz;
using WorldRank.Application.Commands.CurrencyRates;
using WorldRank.Application.ExchangeRates;
using WorldRank.Application.Jobs;
using Xunit;

namespace WorldRank.UnitTests.Jobs;

public sealed class CurrencyRatesJobTests
{
    [Fact]
    public async Task Execute_WhenEcbRequestSucceeds_SendsUpsertCommand()
    {
        // Arrange
        var ecbHttpClient =
            new Mock<IEcbHttpClient>();

        var sender =
            new Mock<ISender>();

        var logger =
            new Mock<ILogger<CurrencyRatesJob>>();

        var context =
            new Mock<IJobExecutionContext>();

        var cancellationToken =
            CancellationToken.None;

        context
            .SetupGet(x => x.CancellationToken)
            .Returns(cancellationToken);

        IReadOnlyList<CurrencyRateDto> rates =
            new[]
            {
                new CurrencyRateDto(
                    "USD",
                    1.15m,
                    new DateTime(2026, 7, 16)),

                new CurrencyRateDto(
                    "GBP",
                    0.86m,
                    new DateTime(2026, 7, 16))
            };

        ecbHttpClient
            .Setup(x => x.GetLatestRatesAsync(
                cancellationToken))
            .ReturnsAsync(rates);

        UpsertCurrencyRatesCommand?
            capturedCommand = null;

        sender
            .Setup(x => x.Send(
                It.IsAny<UpsertCurrencyRatesCommand>(),
                cancellationToken))
            .Callback<
                UpsertCurrencyRatesCommand,
                CancellationToken>(
                (command, _) =>
                    capturedCommand = command)
            .Returns(Task.CompletedTask);

        var job = new CurrencyRatesJob(
            ecbHttpClient.Object,
            sender.Object,
            logger.Object);

        // Act
        await job.Execute(context.Object);

        // Assert
        Assert.NotNull(capturedCommand);
        Assert.Equal(2, capturedCommand.Rates.Count);

        Assert.Contains(
            capturedCommand.Rates,
            rate =>
                rate.Currency == "USD" &&
                rate.Rate == 1.15m);

        Assert.Contains(
            capturedCommand.Rates,
            rate =>
                rate.Currency == "GBP" &&
                rate.Rate == 0.86m);

        ecbHttpClient.Verify(
            x => x.GetLatestRatesAsync(
                cancellationToken),
            Times.Once);

        sender.Verify(
            x => x.Send(
                It.IsAny<UpsertCurrencyRatesCommand>(),
                cancellationToken),
            Times.Once);
    }

    [Fact]
    public async Task Execute_WhenEcbRequestFails_ThrowsJobExecutionException()
    {
        // Arrange
        var ecbHttpClient =
            new Mock<IEcbHttpClient>();

        var sender =
            new Mock<ISender>();

        var logger =
            new Mock<ILogger<CurrencyRatesJob>>();

        var context =
            new Mock<IJobExecutionContext>();

        context
            .SetupGet(x => x.CancellationToken)
            .Returns(CancellationToken.None);

        var originalException =
            new HttpRequestException(
                "ECB service is unavailable.");

        ecbHttpClient
            .Setup(x => x.GetLatestRatesAsync(
                CancellationToken.None))
            .ThrowsAsync(originalException);

        var job = new CurrencyRatesJob(
            ecbHttpClient.Object,
            sender.Object,
            logger.Object);

        // Act
        var exception =
            await Assert.ThrowsAsync<
                JobExecutionException>(
                () => job.Execute(context.Object));

        // Assert
        Assert.Same(
            originalException,
            exception.InnerException);

        Assert.False(exception.RefireImmediately);

        sender.Verify(
            x => x.Send(
                It.IsAny<UpsertCurrencyRatesCommand>(),
                It.IsAny<CancellationToken>()),
            Times.Never);
    }

    [Fact]
    public async Task Execute_WhenCancelled_RethrowsOperationCanceledException()
    {
        // Arrange
        var ecbHttpClient =
            new Mock<IEcbHttpClient>();

        var sender =
            new Mock<ISender>();

        var logger =
            new Mock<ILogger<CurrencyRatesJob>>();

        var context =
            new Mock<IJobExecutionContext>();

        using var cancellationTokenSource =
            new CancellationTokenSource();

        cancellationTokenSource.Cancel();

        var cancellationToken =
            cancellationTokenSource.Token;

        context
            .SetupGet(x => x.CancellationToken)
            .Returns(cancellationToken);

        ecbHttpClient
            .Setup(x => x.GetLatestRatesAsync(
                cancellationToken))
            .ThrowsAsync(
                new OperationCanceledException(
                    cancellationToken));

        var job = new CurrencyRatesJob(
            ecbHttpClient.Object,
            sender.Object,
            logger.Object);

        // Act
        var action =
            () => job.Execute(context.Object);

        // Assert
        await Assert.ThrowsAnyAsync<
            OperationCanceledException>(action);

        sender.Verify(
            x => x.Send(
                It.IsAny<UpsertCurrencyRatesCommand>(),
                It.IsAny<CancellationToken>()),
            Times.Never);
    }
}
