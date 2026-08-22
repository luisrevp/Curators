using Curators.Domain.Enums;
namespace Curators.Domain.ValueObjects;

public sealed record MediaItem
{
    public string FileName { get; }
    public Uri Url { get; }
    public MediaType MediaType { get; }
    public MediaMetadata Metadata { get; }
    public DateTimeOffset CreatedAt { get; }
    
    private MediaItem(string fileName, Uri url, MediaType mediaType, MediaMetadata metadata, DateTimeOffset createdAt)
    {
        FileName = fileName;
        Url = url;
        MediaType = mediaType;
        Metadata = metadata;
        CreatedAt = createdAt;
    }

    public static MediaItem Create(string fileName, Uri url, MediaType mediaType, MediaMetadata metadata, DateTimeOffset createdAt)
    {
        if (string.IsNullOrEmpty(fileName)) throw new ArgumentException("Filename is not valid!");
        if (!MediaType.IsDefined(mediaType)) throw new ArgumentException($"{mediaType} is not supported as mediaType!");
        if (createdAt == default || createdAt == DateTimeOffset.MinValue) throw new Exception("Creation date is not valid");
        ArgumentNullException.ThrowIfNull(metadata);
        ArgumentNullException.ThrowIfNull(url);

        return new MediaItem(fileName, url, mediaType, metadata, createdAt);
    }
}

// record that supports Media Metadata (duration, size, etc)
public sealed record MediaMetadata(
    long SizeInBytes,
    string MimeType, // represents the media type for browsers (i.e: image/png, text/html, etc.)
    string Format,
    int? Width,
    int? Height,
    TimeSpan? Duration
);