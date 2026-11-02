namespace Curators.Domain.ValueObjects;

public sealed class MediaCollection
{
    public readonly int MaxAmountOfMedia;
    public readonly int DefaultAmountOfMedia = 6;
    private readonly HashSet<MediaItem> _media = new();
    public IReadOnlyCollection<MediaItem> Media => this._media;

    public MediaCollection(int? maxAmountOfMedia = null)
    {
        this.MaxAmountOfMedia = maxAmountOfMedia ?? DefaultAmountOfMedia;
    }

    public bool AddMediaItems(List<MediaItem> mediaItems)
    {
        ArgumentNullException.ThrowIfNull(mediaItems);

        var newMediaItems = mediaItems.DistinctBy(item => item.Metadata).ToList();
        int currentMediaCount = this._media.Count;
        var amountOfItems = currentMediaCount + newMediaItems.Count;

        if (amountOfItems > MaxAmountOfMedia)
        {
            throw new InvalidOperationException($"Can only hold up to {MaxAmountOfMedia} elements!");
        }

        foreach (var mediaItem in newMediaItems)
        {
            AddMedia(mediaItem);
        }

        bool isThereChanges = currentMediaCount > this.Media.Count;
        return isThereChanges;
    }

    public bool RemoveMediaItems(List<MediaItem> mediaItems)
    {
        ArgumentNullException.ThrowIfNull(mediaItems);
        int currentMediaCount = this._media.Count;

        if (currentMediaCount == 0)
        {
            throw new InvalidOperationException("Items don't exist in your collection");
        }

        var mediaItemsToRemove = mediaItems.Distinct();

        foreach (var mediaItem in mediaItemsToRemove)
        {
            RemoveMedia(mediaItem);
        }

        bool isThereChanges = currentMediaCount < this.Media.Count;
        return isThereChanges;
    }

    public bool AddMedia(MediaItem mediaItem)
    {
        if (this._media.Count == MaxAmountOfMedia)
        {
            throw new ArgumentOutOfRangeException($"Can't hold more than {MaxAmountOfMedia} media items");
        }

        return this._media.Add(mediaItem);
    }

    public bool RemoveMedia(MediaItem mediaItem)
    {
        if (this._media.Count == 0)
        {
            throw new InvalidOperationException("The collection of media items is empty. Nothing can be removed");
        }

        return this._media.Remove(mediaItem);
    }
}
