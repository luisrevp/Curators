using Curators.Domain.Enums;

namespace Curators.Domain.ValueObjects;
public sealed record CancellationRule
{
    public TimeSpan MinimumNotice { get; } 
    public decimal RefundPercentage { get; }
    public CancellationActor Actor { get; }

    public CancellationRule(TimeSpan minimumNotice, decimal refundPercentage, CancellationActor actor)
    {
        if (minimumNotice < TimeSpan.Zero)
            throw new ArgumentOutOfRangeException(nameof(minimumNotice));

        if (refundPercentage is (< 0 or > 100))
            throw new ArgumentOutOfRangeException(nameof(refundPercentage));

        if (!Enum.IsDefined(actor))
        {
            throw new ArgumentOutOfRangeException(nameof(actor));
        }

        MinimumNotice = minimumNotice;
        RefundPercentage = refundPercentage;
        Actor = actor;
    }
}