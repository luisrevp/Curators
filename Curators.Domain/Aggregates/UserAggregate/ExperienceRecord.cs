using Curators.Domain.Aggregates.TagsAggregate;
using Curators.Domain.Enums;
using Curators.Domain.SeedWork;
using Curators.Domain.ValueObjects;
namespace Curators.Domain.Aggregates.UserAggregate;
public sealed class ExperienceRecord : Entity<ExperienceRecordId>
{
    private static readonly int MaxLengthDescription = 1000;
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

    public static ExperienceRecord Create(ExperienceRecordParameters experience)
    {
        if (IsDescriptionTooLong(experience.RoleDescription))
        {
            throw new InvalidOperationException($"Description can only hold up to {MaxLengthDescription} characters");
        }

        return new ExperienceRecord(experience);
    }

    public void UpdateDescription(string description)
    {
        if (IsDescriptionTooLong(description))
        {
            throw new ArgumentException($"Description must be less than {MaxLengthDescription} characters");
        }

        this.RoleDescription = description.Trim();
    }

    public void UpdateRoleName(string newRoleName) =>
        this.RoleName = newRoleName ?? throw new ArgumentNullException(nameof(newRoleName));
    public void UpdateLocaction(Location newLocation) => 
        this.Location = newLocation ?? throw new ArgumentNullException(nameof(newLocation));
    public void UpdateOrganization(string newOrganization) => 
        this.OrganizationName = newOrganization ?? throw new ArgumentNullException(nameof(newOrganization));
    public void UpdateDuration(DateRange newDuration)
    {
        if (!newDuration.End.HasValue)
        {
            this.IsCurrentExperience = false;
        }

        this.Duration = newDuration;
    }
    public void MakeExperienceCurrent() => this.IsCurrentExperience = true;
    public void AddTag(TagId tag) => this.Tags?.AddTag(tag);
    public void RemoveTag(TagId tag) => this.Tags?.RemoveTag(tag);
    public void AddTags(List<TagId> tags) => this.Tags?.AddTags(tags);
    public void RemoveTags(List<TagId> tags) => this.Tags?.RemoveTags(tags);
    public void AddMedia(MediaItem mediaItem) => this.Media?.AddMedia(mediaItem);
    public void RemoveMedia(MediaItem mediaItem) => this.Media?.RemoveMedia(mediaItem);
    public void AddMediaItems(List<MediaItem> mediaItems) => this.Media?.AddMediaItems(mediaItems);
    public void RemoveMediaItems(List<MediaItem> mediaItems) => this.Media?.RemoveMediaItems(mediaItems);

    private static bool IsDescriptionTooLong(string description) => description.Trim().Length > MaxLengthDescription;
    
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

    private ExperienceRecordParameters(
        string roleName,
        string roleDescription,
        string organizationName,
        Location location,
        DateRange duration,
        TagCollection? tags = null,
        MediaCollection? media = null,
        bool isCurrentExperience = false
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
}

public readonly record struct ExperienceRecordId(Guid Value)
{
    public static ExperienceRecordId Generate() => new ExperienceRecordId(Guid.NewGuid());
}