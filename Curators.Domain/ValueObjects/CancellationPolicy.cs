using Curators.Domain.Enums;

namespace Curators.Domain.ValueObjects;
public sealed record CancellationPolicy
{
    public CancellationPolicyType Type { get; }
    public IReadOnlyCollection<CancellationRule> Rules { get; }

    public CancellationPolicy(CancellationPolicyType type, IReadOnlyCollection<CancellationRule> rules)    
    {
        ArgumentNullException.ThrowIfNull(rules);

        if (!Enum.IsDefined(type))
        {
            throw new ArgumentOutOfRangeException(
                paramName: nameof(type),
                actualValue: type,
                message: $"{type} is an invalid cancellation policy type"
            );
        }

        var ruleList = rules.ToArray();

        if (ruleList.Length == 0)
        {
            throw new ArgumentOutOfRangeException(
                paramName: nameof(rules),
                message: "You have to provide at least 1 valid cancellation rule"
            );
        };

        Type = type;
        Rules = rules;
    }

    public CancellationRule GetApplicableRule(CancellationActor actor, TimeSpan notice)
    {
        if (this.Type.Equals(CancellationPolicyType.NonRefundable))
        {
            throw new InvalidOperationException("Current policy doens't allow refund");
        }

        if (!Enum.IsDefined(actor))
        {
            throw new ArgumentOutOfRangeException(
                paramName: nameof(actor),
                actualValue: actor,
                message: $"{actor} is an invalid actor type"
            );
        }

        var applicableRule = this.Rules
            .Where(rule => rule.Actor == actor && rule.MinimumNotice <= notice)
            .OrderByDescending(rule => rule.MinimumNotice)
            .FirstOrDefault();

        if (applicableRule is null)
        {
            throw new InvalidOperationException("No cancellation rule applies to the current notice period.");
        }

        return applicableRule;
    }
}