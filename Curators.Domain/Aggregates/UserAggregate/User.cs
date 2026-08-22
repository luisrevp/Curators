using Curators.Domain.Aggregates.JobsAggregate;
using Curators.Domain.SeedWork;
using Curators.Domain.ValueObjects;
namespace Curators.Domain.Aggregates.UserAggregate;

// Inside an aggregate, you generally hold references to the objects themselves, not their IDs.
// So, when loading "User" it carries also its inner entities (the WHOLE aggregate)
public sealed class User : Entity<UserId>, IAggregateRoot
{
    public FullName FullName { get; private set; }
    public Email Email { get; private set; }
    public ProfileDetails ProfileDetails { get; private set; }
    public DateTime DoB { get; private set; }
    public DateTime CreateAt { get; private set; }
    public bool IsActive { get; private set; }

    // Relationships with child Entities in bounded context (for readonly props and fields)
    private readonly List<JobId> _jobsApplied = new(); 

    private readonly List<Project> _portfolio = new();

    private readonly List<ExperienceRecord> _experience = new();
    public IReadOnlyCollection<JobId> JobIds => _jobsApplied.AsReadOnly();
    public IReadOnlyCollection<Project> Portfolio => _portfolio.AsReadOnly();
    public IReadOnlyCollection<ExperienceRecord> Experience => _experience.AsReadOnly();

    public User(CreateUserParameters userParams) : base(UserId.Generate())
    {
        this.FullName = userParams.FullName;
        this.Email = userParams.Email;
        this.ProfileDetails = userParams.ProfileDetails;
        this.DoB = userParams.DoB;
        this.CreateAt = userParams.CreateAt;
        this.IsActive = userParams.IsActive;
    }


    #region Bounded actions for projects: exposed business logic
    public void AddProject(Project project) => _portfolio.Add(project);

    public void DeleteProject(Project project)
    {
        if (!this._portfolio.Contains(project))
        {
            return;
        }

        _portfolio.Remove(project);
    }

    public void AddExperience(ExperienceRecord experience)
    {
        if (experience == null)
        {
            throw new ArgumentNullException(nameof(experience));
        }

        bool areExperiencesCollisioning = this._experience.Any(exp =>
            exp.Duration.OverlapsWith(experience.Duration));

        if (areExperiencesCollisioning)
        {
            throw new InvalidOperationException("Dates from experiences can't overlap");
        }

        this._experience.Add(experience);

    }
    #endregion
}


public sealed record CreateUserParameters(
    FullName FullName,
    ProfileDetails ProfileDetails,
    Email Email,
    DateTime DoB,
    DateTime CreateAt,
    bool IsActive
);

public readonly record struct UserId(Guid Value)
{
    public static UserId Generate() => new UserId(Guid.NewGuid());
}