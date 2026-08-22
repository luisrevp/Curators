using Curators.Domain.SeedWork;
using Curators.Domain.Aggregates.UserAggregate;

namespace Curators.Domain.Aggregates.TagsAggregate;

public sealed class Tag : Entity<TagId>, IAggregateRoot
{
    public string Value { get; private set; }

    private Tag(string value) : base(TagId.Generate())
    {
        Value = value.Trim().ToLowerInvariant();
    }

    public static Tag Create(string value)
    {
        if (!string.IsNullOrWhiteSpace(value) || value.Length < 2)
            throw new ArgumentException("Tag cannot be empty!", nameof(value));

        if (value.Length < 2)
            throw new ArgumentException("Tag should be at least 2 characters!", nameof(value));

        return new Tag(value);
    }

    public void ModifyTag(string newValue)
    {
        if (this.Value == newValue.Trim()) return;

        if (string.IsNullOrWhiteSpace(newValue))
            throw new ArgumentException("You must provide a new value for the tag");

        this.Value = newValue.Trim().ToLowerInvariant();
    }
}

public readonly record struct TagId(Guid Value)
{
    public static TagId Generate() => new TagId(Guid.NewGuid());
}