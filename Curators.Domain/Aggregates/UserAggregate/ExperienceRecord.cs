using Curators.Domain.Aggregates.TagsAggregate;
using Curators.Domain.Enums;
using Curators.Domain.SeedWork;
using Curators.Domain.ValueObjects;
namespace Curators.Domain.Aggregates.UserAggregate;
public sealed class ExperienceRecord : Entity<ExperienceRecordId>
{
    public string OrganizationName { get; private set; }
    public string RoleName { get; private set; }
    public string RoleDescription { get; private set; }
    public Location Location { get; private set; }
    public bool IsCurrentExperience { get; private set; }
    public DateRange Duration { get; private set; }
    public TagCollection? Tags { get; private set; }
    public MediaCollection? Media { get; private set; }

    private ExperienceRecord(
        ExperienceRecordParameters expParams
    ) : base(ExperienceRecordId.Generate())
    {
        this.OrganizationName = expParams.OrganizationName;
        this.RoleName = expParams.RoleName;
        this.RoleDescription = expParams.RoleDescription;
        this.IsCurrentExperience = expParams.IsCurrentExperience;
        this.Location = expParams.Location;
        this.Duration = expParams.Duration;
        this.Tags = expParams.Tags;
        this.Media = expParams.Media;
    }

    public static ExperienceRecord Create(ExperienceRecordParameters experience) => new ExperienceRecord(experience);
}

public sealed record ExperienceRecordParameters
{
    public string OrganizationName { get; }
    public string RoleName { get; }
    public string RoleDescription { get; }
    public bool IsCurrentExperience { get; }
    public Location Location { get; }
    public DateRange Duration { get; }
    public TagCollection Tags { get; }
    public MediaCollection Media { get; }

    public ExperienceRecordParameters(
        bool isCurrentExperience,
        string organizationName,
        string roleDescription,
        string roleName,
        Location location,
        DateRange duration,
        TagCollection? tags = null,
        MediaCollection? media = null
    )
    {
        this.IsCurrentExperience = isCurrentExperience;
        this.OrganizationName = string.IsNullOrEmpty(organizationName) ? throw new ArgumentException(nameof(organizationName)) : organizationName;
        this.RoleDescription =  string.IsNullOrEmpty(roleDescription) ? throw new ArgumentException(nameof(roleDescription)) : roleDescription;
        this.RoleName = string.IsNullOrEmpty(roleName) ? throw new ArgumentException(nameof(roleName)) : roleName;
        this.Location = location ?? throw new ArgumentException("Location is required");
        this.Duration = duration ?? throw new ArgumentException("Duration Is Required");
        this.Tags = tags ?? new(maxAmountOfTags: 8);
        this.Media = media ?? new(maxAmountOfMedia: 8);
    }

    public void AddTag(TagId tag) => this.Tags.AddTag(tag);
    public void RemoveTag(TagId tag) => this.Tags.RemoveTag(tag);
    public void AddTags(List<TagId> tags) => this.Tags.AddTags(tags);
    public void RemoveTags(List<TagId> tags) => this.Tags.RemoveTags(tags);
    public void AddMedia(MediaItem mediaItem) => this.Media.AddMedia(mediaItem);
    public void RemoveMedia(MediaItem mediaItem) => this.Media.RemoveMedia(mediaItem);
    public void AddMediaItems(List<MediaItem> mediaItems) => this.Media.AddMediaItems(mediaItems);
    public void RemoveMediaItems(List<MediaItem> mediaItems) => this.Media.RemoveMediaItems(mediaItems);
}

public readonly record struct ExperienceRecordId(Guid Value)
{
    public static ExperienceRecordId Generate() => new ExperienceRecordId(Guid.NewGuid());
}