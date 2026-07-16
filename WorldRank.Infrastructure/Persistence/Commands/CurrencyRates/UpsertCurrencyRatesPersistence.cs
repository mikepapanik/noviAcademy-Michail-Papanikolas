using WorldRank.Application.Infrastructure.CurrencyRates;
using WorldRank.Application.Interfaces;
using CurrencyRatesAggregate =
    WorldRank.Domain.CurrencyRates.CurrencyRates;

namespace WorldRank.Infrastructure.Persistence.Commands.CurrencyRates;

public sealed class UpsertCurrencyRatesPersistence
    : IUpsertCurrencyRatesPersistence
{
    private readonly ICurrencyRatesRepository
        _currencyRatesRepository;

    public UpsertCurrencyRatesPersistence(
        ICurrencyRatesRepository currencyRatesRepository)
    {
        _currencyRatesRepository =
            currencyRatesRepository;
    }

    public async Task UpsertAsync(
        IReadOnlyCollection<CurrencyRatesAggregate> currencyRates,
        CancellationToken cancellationToken)
    {
        await _currencyRatesRepository.UpsertAsync(
            currencyRates,
            cancellationToken);
    }
}
