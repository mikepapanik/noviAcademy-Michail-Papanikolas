using MediatR;
using WorldRank.Application.ExchangeRates;

namespace WorldRank.Application.Commands.CurrencyRates;

public sealed record UpsertCurrencyRatesCommand(
    IReadOnlyCollection<CurrencyRateDto> Rates)
    : IRequest;
