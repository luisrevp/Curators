using Curators.Domain.Enums;
using Curators.Domain.SeedWork;

namespace Curators.Domain.ValueObjects;

public sealed record ProfileDetails
{
    public Address? Address { get; init; }
    public MediaItem? ProfilePicture { get; init; }
    public string Introduction { get; init; }
    public List<Language>? Languages { get; init; }
    public Pronouns? Pronouns { get; init; }

    private ProfileDetails(Address? address, MediaItem? profilePicture, Pronouns? pronouns, string introduction, List<Language> languages)
    {
        this.Address = address;
        this.ProfilePicture = profilePicture;
        this.Pronouns = pronouns;
        this.Introduction = introduction;
        this.Languages = languages;
    }

    public static ProfileDetails Create(
        Address? address,
        MediaItem? profilePicture,
        Pronouns? pronouns = null,
        string? introduction = null,
        List<Language>? languages = null
    )
    {
        return new ProfileDetails(
            address: address,
            profilePicture: profilePicture,
            pronouns: pronouns,
            introduction: introduction ?? string.Empty,
            languages: languages ?? new List<Language>()
        );
    }

}

