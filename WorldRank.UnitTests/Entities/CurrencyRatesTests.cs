using WorldRank.Domain.CurrencyRates;

namespace WorldRank.UnitTests.Entities;

public class CurrencyRatesTests
{
    [Fact]
    public void Constructor_ValidValues_InitializesAllPropertiesCorrectly()
    {
        // Arrange
        const string currency = "EUR";
        const decimal rate = 1.25m;
        var date = new DateTime(2026, 7, 16);

        // Act
        var currencyRate = new CurrencyRates(
            currency,
            rate,
            date);

        // Assert
        Assert.Equal(currency, currencyRate.Currency);
        Assert.Equal(rate, currencyRate.Rate);
        Assert.Equal(date, currencyRate.Date);
    }

    [Fact]
    public void Constructor_CurrencyWithWhitespaceAndLowercase_NormalizesCurrency()
    {
        // Arrange
        const string currency = "  eur  ";

        // Act
        var currencyRate = new CurrencyRates(
            currency,
            1.25m,
            new DateTime(2026, 7, 16));

        // Assert
        Assert.Equal("EUR", currencyRate.Currency);
    }

    [Fact]
    public void Constructor_DateWithTime_StoresDateWithoutTime()
    {
        // Arrange
        var date = new DateTime(
            2026,
            7,
            16,
            14,
            30,
            45);

        // Act
        var currencyRate = new CurrencyRates(
            "EUR",
            1.25m,
            date);

        // Assert
        Assert.Equal(date.Date, currencyRate.Date);
        Assert.Equal(TimeSpan.Zero, currencyRate.Date.TimeOfDay);
    }

    [Theory]
    [InlineData(null)]
    [InlineData("")]
    [InlineData(" ")]
    [InlineData("   ")]
    public void Constructor_NullOrWhitespaceCurrency_ThrowsArgumentException(
        string? currency)
    {
        // Act
        void CreateCurrencyRate()
        {
            _ = new CurrencyRates(
                currency!,
                1.25m,
                new DateTime(2026, 7, 16));
        }

        // Assert
        var exception = Assert.Throws<ArgumentException>(
            CreateCurrencyRate);

        Assert.Equal("currency", exception.ParamName);
    }

    [Theory]
    [InlineData(0)]
    [InlineData(-1)]
    [InlineData(-100)]
    public void Constructor_NonPositiveRate_ThrowsArgumentOutOfRangeException(
        int rate)
    {
        // Act
        void CreateCurrencyRate()
        {
            _ = new CurrencyRates(
                "EUR",
                rate,
                new DateTime(2026, 7, 16));
        }

        // Assert
        var exception =
            Assert.Throws<ArgumentOutOfRangeException>(
                CreateCurrencyRate);

        Assert.Equal("rate", exception.ParamName);
    }

    [Fact]
    public void UpdateRate_PositiveRate_UpdatesRate()
    {
        // Arrange
        var currencyRate = new CurrencyRates(
            "EUR",
            1.25m,
            new DateTime(2026, 7, 16));

        const decimal updatedRate = 1.50m;

        // Act
        currencyRate.UpdateRate(updatedRate);

        // Assert
        Assert.Equal(updatedRate, currencyRate.Rate);
    }

    [Theory]
    [InlineData(0)]
    [InlineData(-1)]
    [InlineData(-100)]
    public void UpdateRate_NonPositiveRate_ThrowsArgumentOutOfRangeException(
        int rate)
    {
        // Arrange
        const decimal originalRate = 1.25m;

        var currencyRate = new CurrencyRates(
            "EUR",
            originalRate,
            new DateTime(2026, 7, 16));

        // Act
        void UpdateRate()
        {
            currencyRate.UpdateRate(rate);
        }

        // Assert
        var exception =
            Assert.Throws<ArgumentOutOfRangeException>(
                UpdateRate);

        Assert.Equal("rate", exception.ParamName);
        Assert.Equal(originalRate, currencyRate.Rate);
    }
}