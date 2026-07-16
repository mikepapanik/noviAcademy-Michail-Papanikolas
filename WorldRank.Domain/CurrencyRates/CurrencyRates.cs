namespace WorldRank.Domain.CurrencyRates;

public sealed class CurrencyRates
{
    private CurrencyRates()
    {
    }

    public CurrencyRates(
        string currency,
        decimal rate,
        DateTime date)
    {
        if (string.IsNullOrWhiteSpace(currency))
        {
            throw new ArgumentException(
                "Currency is required.",
                nameof(currency));
        }

        Currency = currency
            .Trim()
            .ToUpperInvariant();

        Date = date.Date;

        UpdateRate(rate);
    }

    public string Currency { get; private set; } =
        string.Empty;

    public decimal Rate { get; private set; }

    public DateTime Date { get; private set; }

    public void UpdateRate(decimal rate)
    {
        if (rate <= 0)
        {
            throw new ArgumentOutOfRangeException(
                nameof(rate),
                "Currency rate must be greater than zero.");
        }

        Rate = rate;
    }
}