namespace WorldRank.Application.ExchangeRates;

public sealed record CurrencyRateDto(
    string Currency,
    decimal Rate,
    DateTime Date);
