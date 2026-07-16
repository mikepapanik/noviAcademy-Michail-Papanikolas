using System.Net;
using System.Text;
using WorldRank.Gateway.ExchangeRates;
using WorldRank.UnitTests.Helpers;
using Xunit;

namespace WorldRank.UnitTests.Gateway.ExchangeRates;

public sealed class EcbHttpClientTests
{
    private const string ExpectedUrl =
        "https://www.ecb.europa.eu/stats/eurofxref/eurofxref-daily.xml";

    [Fact]
    public async Task GetLatestRatesAsync_ValidXml_ReturnsMappedRates()
    {
        // Arrange
        const string xml = """
            <?xml version="1.0" encoding="UTF-8"?>
            <gesmes:Envelope
                xmlns:gesmes="http://www.gesmes.org/xml/2002-08-01"
                xmlns="http://www.ecb.int/vocabulary/2002-08-01/eurofxref">
                <gesmes:subject>Reference rates</gesmes:subject>
                <Cube>
                    <Cube time="2026-07-16">
                        <Cube currency="USD" rate="1.1500"/>
                        <Cube currency="GBP" rate="0.8600"/>
                    </Cube>
                </Cube>
            </gesmes:Envelope>
            """;

        var messageHandler =
            new StubHttpMessageHandler(
                (_, _) =>
                {
                    var response =
                        new HttpResponseMessage(
                            HttpStatusCode.OK)
                        {
                            Content = new StringContent(
                                xml,
                                Encoding.UTF8,
                                "application/xml")
                        };

                    return Task.FromResult(response);
                });

        using var httpClient =
            new HttpClient(messageHandler);

        var client =
            new EcbHttpClient(httpClient);

        // Act
        var rates = await client.GetLatestRatesAsync(
            CancellationToken.None);

        // Assert
        Assert.Equal(2, rates.Count);

        var usd = Assert.Single(
            rates,
            rate => rate.Currency == "USD");

        Assert.Equal(1.1500m, usd.Rate);
        Assert.Equal(
            new DateTime(2026, 7, 16),
            usd.Date);

        var gbp = Assert.Single(
            rates,
            rate => rate.Currency == "GBP");

        Assert.Equal(0.8600m, gbp.Rate);
        Assert.Equal(
            new DateTime(2026, 7, 16),
            gbp.Date);

        Assert.Equal(1, messageHandler.CallCount);

        Assert.NotNull(messageHandler.LastRequest);

        Assert.Equal(
            HttpMethod.Get,
            messageHandler.LastRequest.Method);

        Assert.Equal(
            ExpectedUrl,
            messageHandler.LastRequest.RequestUri?.ToString());
    }

    [Fact]
    public async Task GetLatestRatesAsync_UnsuccessfulResponse_ThrowsHttpRequestException()
    {
        // Arrange
        var messageHandler =
            StubHttpMessageHandler.Create(
                HttpStatusCode.BadGateway,
                "ECB service is unavailable.");

        using var httpClient =
            new HttpClient(messageHandler);

        var client =
            new EcbHttpClient(httpClient);

        // Act
        var action =
            () => client.GetLatestRatesAsync(
                CancellationToken.None);

        // Assert
        await Assert.ThrowsAsync<
            HttpRequestException>(action);

        Assert.Equal(1, messageHandler.CallCount);
    }

    [Fact]
    public async Task GetLatestRatesAsync_InvalidXml_ThrowsInvalidOperationException()
    {
        // Arrange
        const string invalidXml = """
            <invalid>
                <unclosed>
            </invalid>
            """;

        var messageHandler =
            StubHttpMessageHandler.Create(
                HttpStatusCode.OK,
                invalidXml);

        using var httpClient =
            new HttpClient(messageHandler);

        var client =
            new EcbHttpClient(httpClient);

        // Act
        var action =
            () => client.GetLatestRatesAsync(
                CancellationToken.None);

        // Assert
        await Assert.ThrowsAsync<
            InvalidOperationException>(action);

        Assert.Equal(1, messageHandler.CallCount);
    }

    [Fact]
    public async Task GetLatestRatesAsync_CancelledRequest_ThrowsOperationCanceledException()
    {
        // Arrange
        var messageHandler =
            new StubHttpMessageHandler(
                async (_, cancellationToken) =>
                {
                    await Task.Delay(
                        Timeout.Infinite,
                        cancellationToken);

                    return new HttpResponseMessage(
                        HttpStatusCode.OK);
                });

        using var httpClient =
            new HttpClient(messageHandler);

        var client =
            new EcbHttpClient(httpClient);

        using var cancellationTokenSource =
            new CancellationTokenSource();

        cancellationTokenSource.Cancel();

        // Act
        var action =
            () => client.GetLatestRatesAsync(
                cancellationTokenSource.Token);

        // Assert
        await Assert.ThrowsAnyAsync<
            OperationCanceledException>(action);
    }
}
