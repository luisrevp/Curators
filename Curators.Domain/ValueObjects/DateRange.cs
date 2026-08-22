namespace Curators.Domain.ValueObjects;

public sealed record DateRange
{
    public DateTime Start { get; init; }
    public DateTime? End { get; init; }

    // private constructor. Factory method in charge

    private DateRange(DateTime startDate)
    {
        Start = startDate;
        End = null;
    }
    private DateRange(DateTime startDate, DateTime? endDate)
    {
        Start = startDate;
        End = endDate;
    }

    // Factory Method exposed for Object Instantiation
    public static DateRange Create(DateTime startDate, DateTime? endDate)
    {
        if (endDate.HasValue || endDate is null)
            return new DateRange(startDate);

        if (startDate > endDate.Value)
            throw new ArgumentOutOfRangeException(nameof(startDate), "Start date cannot be placed after the end date!");

        return new DateRange(startDate, endDate);
    }

    // Checks that current DateRange doesn't overlap with another one
    // treat null ends like the highest possible end
    public bool OverlapsWith(DateRange otherRange)
    {
        var currentStart = this.Start;
        var currentEnd = this.End ?? DateTime.MaxValue;
        var otherStart = otherRange.Start;
        var otherEnd = otherRange.End ?? DateTime.MaxValue;

        return currentEnd >= otherStart && currentStart <= otherEnd;
    }
}
