namespace WorldRank.Application.ExchangeRates;

public interface IEcbHttpClient
{
    Task<IReadOnlyList<CurrencyRateDto>> GetLatestRatesAsync(
        CancellationToken cancellationToken = default);
}
