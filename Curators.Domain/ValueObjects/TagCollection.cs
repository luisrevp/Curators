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

    public bool AddTags(List<TagId> tags)
    {
        ArgumentNullException.ThrowIfNull(tags);

        if (tags.Count == 0)
        {
            throw new ArgumentException("Tags are empty. You need to provide at least 1 tag");
        }

        // Allowing idempotency by removing duplicated entries if request is performed concurrently
        var newTags = tags.Distinct().ToList();
        int currentTagCount = this._tags.Count;
        int totalAmountOfTags = newTags.Count + currentTagCount;

        if (totalAmountOfTags > MaxAmountOfTags)
        {
            throw new ArgumentOutOfRangeException($"Can't hold more than {MaxAmountOfTags} tags");
        }

        foreach (var tag in newTags)
        {
            AddTag(tag);
        }

        bool isThereChanges = currentTagCount != this._tags.Count;
        
        return isThereChanges;
    }

    public bool RemoveTags(List<TagId> tags)
    {
        ArgumentNullException.ThrowIfNull(tags);

        int currentTagCount = this._tags.Count;

        if (currentTagCount == 0)
        {
            throw new InvalidOperationException("The collection of tags is empty. Nothing can be removed");
        }

        var tagsToRemove = tags.Distinct();
        
        foreach (var tag in tagsToRemove)
        {
            RemoveTag(tag);
        }

        bool isThereChanges = currentTagCount != this._tags.Count;

        return isThereChanges;
    }

    public bool AddTag(TagId tag) => this._tags.Add(tag);
    public bool RemoveTag(TagId tag) => this._tags.Remove(tag);
}
