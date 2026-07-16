using Microsoft.EntityFrameworkCore;
using WorldRank.Application.Interfaces;
using WorldRank.Infrastructure.Persistence.Context;
using CurrencyRatesAggregate =
    WorldRank.Domain.CurrencyRates.CurrencyRates;

namespace WorldRank.Infrastructure.Repositories;

public class DBCurrencyRatesRepository
    : ICurrencyRatesRepository
{
    private readonly WorldRankDbContext _dbContext;

    public DBCurrencyRatesRepository(
        WorldRankDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task UpsertAsync(
        IReadOnlyCollection<CurrencyRatesAggregate> currencyRates,
        CancellationToken cancellationToken)
    {
        if (currencyRates.Count == 0)
        {
            return;
        }

        var currencies = currencyRates
            .Select(currencyRate =>
                currencyRate.Currency)
            .Distinct()
            .ToArray();

        var dates = currencyRates
            .Select(currencyRate =>
                currencyRate.Date)
            .Distinct()
            .ToArray();

        var existingCurrencyRates =
            await _dbContext.CurrencyRates
                .Where(existing =>
                    currencies.Contains(existing.Currency) &&
                    dates.Contains(existing.Date))
                .ToListAsync(cancellationToken);

        var existingByKey = existingCurrencyRates
            .ToDictionary(currencyRate => (
                currencyRate.Currency,
                currencyRate.Date));

        foreach (var currencyRate in currencyRates)
        {
            var key = (
                currencyRate.Currency,
                currencyRate.Date);

            if (existingByKey.TryGetValue(
                    key,
                    out var existingCurrencyRate))
            {
                existingCurrencyRate.UpdateRate(
                    currencyRate.Rate);

                continue;
            }

            await _dbContext.CurrencyRates.AddAsync(
                currencyRate,
                cancellationToken);

            existingByKey.Add(
                key,
                currencyRate);
        }

        await _dbContext.SaveChangesAsync(
            cancellationToken);
    }
}
