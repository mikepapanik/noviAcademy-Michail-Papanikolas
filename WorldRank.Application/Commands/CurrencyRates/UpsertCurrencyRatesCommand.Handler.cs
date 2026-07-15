using MediatR;
using WorldRank.Application.Infrastructure.CurrencyRates;
using CurrencyRatesAggregate =
    WorldRank.Domain.CurrencyRates.CurrencyRates;

namespace WorldRank.Application.Commands.CurrencyRates;

public sealed class UpsertCurrencyRatesCommandHandler
    : IRequestHandler<UpsertCurrencyRatesCommand>
{
    private readonly IUpsertCurrencyRatesPersistence
        _upsertCurrencyRatesPersistence;

    public UpsertCurrencyRatesCommandHandler(
        IUpsertCurrencyRatesPersistence
            upsertCurrencyRatesPersistence)
    {
        _upsertCurrencyRatesPersistence =
            upsertCurrencyRatesPersistence;
    }

    public async Task Handle(
        UpsertCurrencyRatesCommand request,
        CancellationToken cancellationToken)
    {
        if (request.Rates.Count == 0)
        {
            return;
        }

        var currencyRates = request.Rates
            .Select(rate => new CurrencyRatesAggregate(
                rate.Currency,
                rate.Rate,
                rate.Date))
            .ToArray();

        await _upsertCurrencyRatesPersistence.UpsertAsync(
            currencyRates,
            cancellationToken);
    }
}
