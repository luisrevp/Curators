namespace Curators.Domain.ValueObjects;

public sealed record Money
{
    public decimal Amount { get; }
    public string Currency { get; }

    private Money(decimal amount, string currency)
    {
        Amount = amount;
        Currency = currency.ToUpperInvariant();
    }

    public static Money Create(decimal amount, string currency)
    {
        if (amount < 0)
            throw new ArgumentOutOfRangeException(nameof(amount), "The amount can't be negative");

        if (string.IsNullOrWhiteSpace(currency))
            throw new ArgumentException($"{nameof(currency)}: the Currency is not valid");

        return new Money(amount, currency);
    }

    public Money Add(Money otherMoney)
        => otherMoney.Currency != this.Currency
            ? throw new ArgumentException("Currencies must match!")
            : Create(this.Amount + otherMoney.Amount, otherMoney.Currency);

    public Money Substract(Money otherMoney)
        => otherMoney.Currency != this.Currency
            ? throw new ArgumentException("Currencies must match!")
            : Create(this.Amount - otherMoney.Amount, otherMoney.Currency);

    // normally used for tax rates, discounts, percentages, etc.
    public Money Multiply(Money otherMoney)
        => otherMoney.Currency != this.Currency
            ? throw new ArgumentException("Currencies must match!")
            : Create(this.Amount * otherMoney.Amount, otherMoney.Currency);

    // used for splitting amount without losing cents
    public Money Allocate(Money otherMoney)
        => otherMoney.Currency != this.Currency
            ? throw new ArgumentException("Currencies must match!")
            : Create((decimal)(this.Amount / otherMoney.Amount), otherMoney.Currency);
}
