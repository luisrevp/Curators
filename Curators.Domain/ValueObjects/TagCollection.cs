using Curators.Domain.Aggregates.TagsAggregate;

namespace Curators.Domain.ValueObjects;

public sealed class TagCollection
{
    public readonly int MaxAmountOfTags;
    public readonly int DefaultAmountOfTags = 10;
    private readonly HashSet<TagId> _tags = new();
    public IReadOnlyCollection<TagId> Tags => this._tags;

    public TagCollection(int? maxAmountOfTags = null)
    {
        this.MaxAmountOfTags = maxAmountOfTags ?? DefaultAmountOfTags;
    }

    public void AddTags(List<TagId> tags)
    {
        ArgumentNullException.ThrowIfNull(tags);

        if (tags.Count == 0)
        {
            throw new ArgumentException("Tags are empty. You need to provide at least 1 tag");
        }

        // Allowing idempotency by removing duplicated entries if request is performed concurrently
        var newTags = tags.Distinct().ToList();
        int totalAmountOfTags = newTags.Count + this._tags.Count;

        if (totalAmountOfTags > MaxAmountOfTags)
        {
            throw new InvalidOperationException("The total amount of tags exceeds the current threshold {}");
        }

        foreach (var tag in newTags)
        {
            AddTag(tag);
        }
    }

    public void RemoveTags(List<TagId> tags)
    {
        ArgumentNullException.ThrowIfNull(tags);

        if (this._tags.Count == 0)
        {
            throw new InvalidOperationException("The collection of tags is empty. Nothing can be removed");
        }

        var tagsToRemove = tags.Distinct();

        foreach (var tag in tagsToRemove)
        {
            RemoveTag(tag);
        }
    }

    public void AddTag(TagId tag)
    {
        if (!this._tags.Add(tag))
        {
            Console.WriteLine("Tag duplicated");
            return;
        }

        Console.WriteLine("Tag added successfully!");
    }

    public void RemoveTag(TagId tag)
    {
        if (!this._tags.Remove(tag))
        {
            Console.WriteLine("Tag doesn't exist in your collection");
            return;
        }

        Console.WriteLine("Tag removed successfully!");
    }
}
