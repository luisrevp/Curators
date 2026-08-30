using Curators.Domain.Aggregates.TagsAggregate;
using Curators.Domain.Aggregates.UserAggregate;
using Curators.Domain.Enums;
using Curators.Domain.SeedWork;
using Curators.Domain.ValueObjects;
using System.Xml.Linq;

namespace Curators.Domain.Aggregates.SerivceAggregate;
public sealed class Service : Entity<ServiceId>, IAggregateRoot
{
    public static readonly int MaxLengthName = 100;
    public static readonly int MaxLengthDescription = 2500;
    public static readonly int MaxAmountOfTags = 6;
    public UserId ProviderId { get; private set; }
    public string Name { get; private set; }
    public string Description { get; private set; }
    public Pricing Pricing { get; private set; }
    public CancellationPolicy CancellationPolicy { get; private set; }
    public ServiceStatus Status { get; private set; }
    public TagCollection Tags { get; private set; }
    public DateTime CreatedAt { get; private set; }
    public DateTime UpdatedAt { get; private set; }
 
    // TODO: Availability logic
    
    private Service(
        UserId providerId,
        string name,
        string description,
        Pricing pricing, 
        CancellationPolicy cancellationPolicy,
        ServiceStatus status,
        TagCollection tags
    ) : base(ServiceId.Generate())
    {
        this.ProviderId = providerId;
        this.Name = name;
        this.Description = description;
        this.Pricing = pricing;
        this.CancellationPolicy = cancellationPolicy;
        this.Status = status;
        this.Tags = tags;
        this.CreatedAt = DateTime.UtcNow;
        this.UpdatedAt = DateTime.UtcNow;
    }

    public static Service Create(
        UserId providerId,
        string name,
        string description,
        Pricing pricing,
        CancellationPolicy cancellationPolicy,
        ServiceStatus? status = null,
        TagCollection? tags = null
    )
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(name, nameof(name));
        ArgumentException.ThrowIfNullOrWhiteSpace(description, nameof(description));
        ArgumentNullException.ThrowIfNull(cancellationPolicy);

        IsNameValid(ref name);
        IsDescriptionValid(ref description);
        
        return new Service(
            providerId, 
            name, 
            description, 
            pricing, 
            cancellationPolicy,
            status ?? ServiceStatus.Draft, 
            tags ?? new(MaxAmountOfTags)
        );
    }

    public void ChangeName(string name)
    {
        if (this.Status == ServiceStatus.Archived)
        {
            throw new InvalidOperationException("Cannot add tags for an archived service.");
        }

        IsNameValid(ref name);
        this.Name = name;
        this.UpdatedAt = DateTime.UtcNow;
    }

    public void ChangeDescription(ref string description)
    {
        if (this.Status == ServiceStatus.Archived)
        {
            throw new InvalidOperationException("Cannot add tags for an archived service.");
        }

        IsDescriptionValid(ref description);
        this.Description = description;
        this.UpdatedAt = DateTime.UtcNow;
    }

    public void UpdatePricing(Pricing newPrice)
    {
        if (this.Status == ServiceStatus.Archived)
        {
            throw new InvalidOperationException("Cannot add tags for an archived service.");
        }

        if (this.Pricing.Equals(newPrice))
        {
            return;
        }

        this.Pricing = newPrice;
        this.UpdatedAt = DateTime.UtcNow;
    }

    public void UpdateCancellationPolicy(CancellationPolicy newCancellationPolicy)
    {
        ArgumentNullException.ThrowIfNull(newCancellationPolicy);

        if (this.Status is not (ServiceStatus.Paused or ServiceStatus.Archived))
        {
            throw new InvalidOperationException($"Cannot update cancellation policy for a service in state \"{this.Status}\"");
        }

        this.CancellationPolicy = newCancellationPolicy;
    }

    public void Draft()
    {
        this.Status = ServiceStatus.Draft;
        this.UpdatedAt = DateTime.UtcNow;
    }

    public void Activate()
    {
        this.Status = ServiceStatus.Active;
        this.UpdatedAt = DateTime.UtcNow;
    }

    public void Pause()
    {
        this.Status = ServiceStatus.Paused;
        this.UpdatedAt = DateTime.UtcNow;
    }

    public void Archive()
    {
        this.Status = ServiceStatus.Archived;
        this.UpdatedAt = DateTime.UtcNow;
    }

    public void AddTag(TagId tag)
    {
        if (this.Status == ServiceStatus.Archived)
        {
            throw new InvalidOperationException("Cannot add tags for an archived service.");
        }

        if (!this.Tags.AddTag(tag))
        {
            return;
        }

        this.UpdatedAt = DateTime.UtcNow;
    }

    public void RemoveTag(TagId tag)
    {
        if (this.Status == ServiceStatus.Archived)
        {
            throw new InvalidOperationException("Cannot remove tags for an archived service.");
        }

        if (this.Tags.RemoveTag(tag))
        {
            return;
        }

        this.UpdatedAt = DateTime.UtcNow;
    }

    public void AddTags(List<TagId> tags)
    {
        if (this.Status == ServiceStatus.Archived)
        {
            throw new InvalidOperationException("Cannot add tags for an archived service.");
        }

        if (!this.Tags.AddTags(tags))
        {
            return;
        }

        this.UpdatedAt = DateTime.UtcNow;
    }

    public void RemoveTags(List<TagId> tags)
    {
        if (this.Status == ServiceStatus.Archived)
        {
            throw new InvalidOperationException("Cannot add tags for an archived service.");
        }

        this.Tags.RemoveTags(tags);
        this.UpdatedAt = DateTime.UtcNow;
    }

    #region private methods
    private static void IsNameValid(ref string name)
    {
        name = name.Trim();
        if (name.Length > MaxLengthName)
        {
            throw new ArgumentOutOfRangeException(
                paramName: nameof(name),
                actualValue: name,
                message: $"Service name can't hold more than {MaxLengthName} characters"
            );
        }
    }

    private static void IsDescriptionValid(ref string description)
    {
        description = description.Trim();
        if (description.Length > MaxLengthDescription)
        {
            throw new ArgumentOutOfRangeException(
                paramName: nameof(description),
                actualValue: description,
                message: $"Service description can't hold more than {MaxLengthDescription} characters"
            );
        }
    }
    #endregion
}

public readonly record struct ServiceId(Guid Value)
{
    public static ServiceId Generate() => new ServiceId(Guid.NewGuid());
}
