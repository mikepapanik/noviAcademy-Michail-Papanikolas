using CurrencyRatesAggregate =
    WorldRank.Domain.CurrencyRates.CurrencyRates;

namespace WorldRank.Application.Interfaces;

public interface ICurrencyRatesRepository
{
    Task UpsertAsync(
        IReadOnlyCollection<CurrencyRatesAggregate> currencyRates,
        CancellationToken cancellationToken);
}
