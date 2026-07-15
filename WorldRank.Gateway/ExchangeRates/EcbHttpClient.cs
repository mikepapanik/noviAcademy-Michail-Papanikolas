using System.Xml.Serialization;
using WorldRank.Application.ExchangeRates;

namespace WorldRank.Gateway.ExchangeRates;

public sealed class EcbHttpClient : IEcbHttpClient
{
    private const string DailyRatesUrl =
        "https://www.ecb.europa.eu/stats/eurofxref/eurofxref-daily.xml";

    private readonly HttpClient _httpClient;

    public EcbHttpClient(HttpClient httpClient)
    {
        _httpClient = httpClient;
    }

    public async Task<IReadOnlyList<CurrencyRateDto>> GetLatestRatesAsync(
        CancellationToken cancellationToken = default)
    {
        using var response = await _httpClient.GetAsync(
            DailyRatesUrl,
            HttpCompletionOption.ResponseHeadersRead,
            cancellationToken);

        response.EnsureSuccessStatusCode();

        await using var stream =
            await response.Content.ReadAsStreamAsync(cancellationToken);

        var serializer = new XmlSerializer(typeof(ResponseDto));

        var responseDto = serializer.Deserialize(stream) as ResponseDto
            ?? throw new InvalidOperationException(
                "The ECB response could not be deserialized.");

        var ratesCube = responseDto.Cube.Rates;

        var rates = ratesCube.Rates
            .Select(rate => new CurrencyRateDto(
                rate.Currency,
                rate.Rate,
                ratesCube.Date))
            .ToArray();

        return rates;
    }
}