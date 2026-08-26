using Curators.Domain.Enums;

namespace Curators.Domain.ValueObjects;
public sealed record Pricing
{
    public Money Amount { get; }
    public decimal Quantity { get; }
    public PricingUnit PricingUnit { get; }
    
    private Pricing(Money amount, decimal quantity, PricingUnit pricingUnit)
    {
        Amount = amount;
        Quantity = quantity;
        PricingUnit = pricingUnit;
    }

    public static Pricing Create(Money amount, decimal quantity, PricingUnit pricingUnit)
    {
        ArgumentNullException.ThrowIfNull(amount);
        
        if (quantity <= 0)
        {
            throw new ArgumentOutOfRangeException(
                nameof(quantity),
                quantity,
                "Quantity must be greater than 0!"
            );
        }

        if (!Enum.IsDefined(pricingUnit))
        {
            throw new ArgumentOutOfRangeException(nameof(pricingUnit));
        }

        return new Pricing(amount, quantity, pricingUnit);
    }
}
