using Curators.Domain.Aggregates.TagsAggregate;
using Curators.Domain.SeedWork;
using Curators.Domain.ValueObjects;
namespace Curators.Domain.Aggregates.UserAggregate;

public sealed class Project : Entity<ProjectId>
{
    private readonly int MaxAmountOfTags = 10;
    private readonly int MaxAmountOfMedia = 6;
    public string ProjectName { get; private set; }
    public string Description { get; private set; }
    public DateRange Duration { get; private set; } // start and end date (or "start only")
    public TagCollection Tags { get; private set; }
    public MediaCollection Media { get; private set; }

    private Project(
        string projectName, 
        string description, 
        DateRange duration,
        TagCollection? tags,
        MediaCollection? media
    ) : base(ProjectId.Generate())
    {
        // Id is assigned by the base constructor; cannot set `Id` here because its setter is inaccessible.
        this.ProjectName = projectName;
        this.Description = description;
        this.Duration = duration;
        this.Tags = tags ?? new(6);
        this.Media = media ?? new(10);
    }

    public static Project AddNew(
        string projectName,
        string description,
        DateRange duration,
        TagCollection? tags = null,
        MediaCollection? media = null
    )
    {
        if (string.IsNullOrWhiteSpace(projectName)) throw new ArgumentException("Name is required");
        if (string.IsNullOrWhiteSpace(description)) throw new ArgumentException("Description required");
        ArgumentNullException.ThrowIfNull(duration);

        return new Project(projectName, description, duration, tags, media);
    }

    public void ChangeName(string? newName) =>
        this.ProjectName = newName ?? throw new ArgumentException("You must provide a valid name!");
    public void ChangeDescription(string newDesc) =>
        this.ProjectName = newDesc ?? throw new ArgumentException("You must provide a valid description!");
    public void ChangeDuration(DateRange newDuration) =>
        this.Duration = newDuration ?? throw new ArgumentException("Duration is not valid");

    public void AddTag(TagId tag) => this.Tags.AddTag(tag);
    public void RemoveTag(TagId tag) => this.Tags.RemoveTag(tag);
    public void AddTags(List<TagId> tags) => this.Tags.AddTags(tags);
    public void RemoveTags(List<TagId> tags) => this.Tags.RemoveTags(tags);
    public void AddMedia(MediaItem mediaItem) => this.Media.AddMedia(mediaItem);
    public void RemoveMedia(MediaItem mediaItem) => this.Media.RemoveMedia(mediaItem);
    public void AddMediaItems(List<MediaItem> mediaItems) => this.Media.AddMediaItems(mediaItems);
    public void RemoveMediaItems(List<MediaItem> mediaItems) => this.Media.RemoveMediaItems(mediaItems);
}

public readonly record struct ProjectId(Guid Value)
{
    public static ProjectId Generate() => new ProjectId(Guid.NewGuid());
}
