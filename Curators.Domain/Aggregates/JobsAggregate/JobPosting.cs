using Curators.Domain.SeedWork;
using Curators.Domain.ValueObjects;
using Curators.Domain.Aggregates.UserAggregate;
using Curators.Domain.Aggregates.TagsAggregate;

namespace Curators.Domain.Aggregates.JobsAggregate;

internal class JobPosting : Entity<JobId>, IAggregateRoot
{
    private readonly List<UserId> _applicants = new();
    public UserId PostedBy { get; private set; }
    public string JobTitle { get; private set; }
    public string JobDescription { get; private set; }
    public bool IsActive { get; private set; } = true;
    public DateTime CreatedAt { get; private set; }
    public DateTime UpdatedAt { get; private set; }
    public Money MinimumBudget { get; private set; }
    public Money MaximumBudget { get; private set; }
    public DateRange HiringDates { get; private set; }
    public TagCollection Tags { get; private set; }

    public IReadOnlyCollection<UserId> Applicants => this._applicants.AsReadOnly();

    private JobPosting(
        JobPostingParameters parameters) : base(JobId.Generate())
    {
        this.PostedBy = parameters.PostedBy;
        this.JobTitle = parameters.JobTitle;
        this.JobDescription = parameters.JobDescription;
        this.MinimumBudget = parameters.MinimumBudget;
        this.MaximumBudget = parameters.MaximumBudget;
        this.HiringDates = parameters.HiringDates;
        this.Tags = parameters?.Tags ?? new(maxAmountOfTags: 6);
        this.CreatedAt = DateTime.UtcNow;
    }

    public static JobPosting Create(JobPostingParameters parameters)
    {
        if (parameters.HiringDates.Start < DateTime.UtcNow)
            throw new InvalidOperationException("Hiring dates should be placed after current date");

        if (!parameters.MaximumBudget.Currency.Equals(parameters.MinimumBudget.Currency))
            throw new InvalidOperationException("Currencies don't match for budgets");

        if (parameters.MinimumBudget.Amount > parameters.MaximumBudget.Amount)
            throw new InvalidOperationException("Minimum budget should be greater than maximum budget");

        return new JobPosting(parameters);
    }

    public void DisableJob() => this.IsActive = false;

    public void ChangeMinimumBudget(Money newMinimum)
    {
        if (newMinimum == this.MinimumBudget)
        {
            Console.WriteLine("Budgets are identical");
            return;
        }

        this.MinimumBudget = newMinimum;
    }

    public void ChangeMaximumBudget(Money newMaximum)
    {
        if (newMaximum == this.MaximumBudget)
        {
            Console.WriteLine("Budgets are identical");
            return;
        }

        this.MaximumBudget = newMaximum;
    }

    public void AddAplicant(UserId newApplicant)
    {
        bool isApplicantAlreadyInPool = this._applicants.Contains(newApplicant);

        if (isApplicantAlreadyInPool)
        {
            Console.WriteLine("Participant already applied for this job");
            return;
        }

        this._applicants.Add(newApplicant);
    }

    public void RemoveApplicant(UserId applicant)
    {
        bool isApplicantAlreadyInPool = this._applicants.Contains(applicant);

        if (isApplicantAlreadyInPool)
        {
            // consider using domain event to notify other parts when this happened
            this._applicants.Remove(applicant);
            Console.WriteLine("Applicant removed from participants pool");
        }
    }

    public void AddTag(TagId tag) => this.Tags.AddTag(tag);
    public void RemoveTag(TagId tag) => this.Tags.RemoveTag(tag);
    public void AddTags(List<TagId> tags) => this.Tags.AddTags(tags);
    public void RemoveTags(List<TagId> tags) => this.Tags.RemoveTags(tags);
}

public sealed record JobPostingParameters(
    UserId PostedBy,
    string JobTitle,
    string JobDescription,
    DateRange HiringDates,
    Money MinimumBudget,
    Money MaximumBudget,
    TagCollection? Tags = null
);

public readonly record struct JobId(Guid value)
{
     public static JobId Generate() => new JobId(Guid.NewGuid());
}