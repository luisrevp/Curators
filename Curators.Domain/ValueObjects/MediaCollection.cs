namespace Curators.Domain.ValueObjects;

public sealed class MediaCollection
{
    private readonly int MaxAmountOfMedia;
    private readonly int DefaultAmountOfMedia = 6;
    private readonly List<MediaItem> _media = new();
    public IReadOnlyCollection<MediaItem> Media => _media.AsReadOnly();

    public MediaCollection(int? maxAmountOfMedia = null)
    {
        this.MaxAmountOfMedia = maxAmountOfMedia ?? DefaultAmountOfMedia;
    }

    public void AddMediaItems(List<MediaItem> mediaItems)
    {
        ArgumentNullException.ThrowIfNull(mediaItems);

        var newMediaItems = mediaItems.DistinctBy(item => item.Metadata).ToList();

        var amountOfItems = newMediaItems.Count + this._media.Count;

        if (amountOfItems > MaxAmountOfMedia)
        {
            throw new InvalidOperationException($"Can only hold up to {MaxAmountOfMedia} elements!");
        }

        foreach (var mediaItem in newMediaItems)
        {
            AddMedia(mediaItem);
        }
    }

    public void RemoveMediaItems(List<MediaItem> mediaItems)
    {
        ArgumentNullException.ThrowIfNull(mediaItems);

        if (this._media.Count == 0)
        {
            throw new InvalidOperationException("Items don't exist in your collection");
        }

        var mediaItemsToRemove = mediaItems.Distinct();

        foreach (var mediaItem in mediaItemsToRemove)
        {
            RemoveMedia(mediaItem);
        }
    }

    internal void AddMedia(MediaItem mediaItem)
    {
        if (this._media.Contains(mediaItem))
        {
            Console.WriteLine("You're attempting to add a duplicated value");
            return;
        }

        this._media.Add(mediaItem);
    }
    internal void RemoveMedia(MediaItem mediaItem)
    {
        if (!this._media.Contains(mediaItem))
        {
            Console.WriteLine("You're attempting to delete an item that doesn't exist!");
            return;
        }

        this._media.Remove(mediaItem);
    }
}
