using CurrencyRatesAggregate =
    WorldRank.Domain.CurrencyRates.CurrencyRates;

namespace WorldRank.Application.Infrastructure.CurrencyRates;

public interface IUpsertCurrencyRatesPersistence
{
    Task UpsertAsync(
        IReadOnlyCollection<CurrencyRatesAggregate> currencyRates,
        CancellationToken cancellationToken);
}
