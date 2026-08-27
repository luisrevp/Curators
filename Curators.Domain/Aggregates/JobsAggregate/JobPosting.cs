using Curators.Domain.Aggregates.TagsAggregate;
using Curators.Domain.Aggregates.UserAggregate;
using Curators.Domain.Enums;
using Curators.Domain.SeedWork;
using Curators.Domain.ValueObjects;
using System.Runtime.CompilerServices;

namespace Curators.Domain.Aggregates.JobsAggregate;

internal class JobPosting : Entity<JobId>, IAggregateRoot
{
    private readonly List<UserId> _applicants = new();
    public UserId PostedBy { get; private set; }
    public string JobTitle { get; private set; }
    public string JobDescription { get; private set; }
    public JobStatus Status { get; private set; } = JobStatus.Draft;
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
        this.UpdatedAt = DateTime.UtcNow;
    }

    public static JobPosting Create(JobPostingParameters parameters)
    {
        if (parameters.HiringDates.Start < DateTime.UtcNow)
            throw new ArgumentOutOfRangeException("Hiring dates should be placed after current date");

        if (parameters.MinimumBudget.Amount > parameters.MaximumBudget.Amount)
            throw new ArgumentOutOfRangeException("Minimum budget should be greater than maximum budget");

        if (!parameters.MaximumBudget.Currency.Equals(parameters.MinimumBudget.Currency))
            throw new InvalidOperationException("Currencies don't match for budgets");

        return new JobPosting(parameters);
    }

    public void Draft()
    {
        this.Status = JobStatus.Draft;
        this.UpdatedAt = DateTime.UtcNow;
    }

    public void Active()
    {
        this.Status = JobStatus.Active;
        this.UpdatedAt = DateTime.UtcNow;
    }

    public void ChangeMinimumBudget(Money newMinimum)
    {
        if (this.Status is not (JobStatus.Paused or JobStatus.Draft))
        {
            throw new InvalidOperationException($"Cannot change budget on a job if state is '{this.Status}'");
        }

        if (newMinimum.Equals(this.MinimumBudget))
        {
            return;
        }

        this.MinimumBudget = newMinimum;
        this.UpdatedAt = DateTime.UtcNow;
    }

    public void ChangeMaximumBudget(Money newMaximum)
    {
        if (this.Status is not (JobStatus.Paused or JobStatus.Draft))
        {
            throw new InvalidOperationException($"Cannot change budget on a job if state is '{this.Status}'");
        }

        if (newMaximum.Equals(this.MaximumBudget))
        {
            return;
        }

        this.MaximumBudget = newMaximum;
        this.UpdatedAt = DateTime.UtcNow;
    }

    public void AddAplicant(UserId newApplicant)
    {
        if (this.Status is not JobStatus.Active)
        {
            throw new InvalidOperationException($"Cannot add applicant if job state is '{this.Status}'");
        }

        bool isApplicantAlreadyInPool = this._applicants.Contains(newApplicant);

        if (isApplicantAlreadyInPool)
        {
            return;
        }

        this._applicants.Add(newApplicant);
        this.UpdatedAt = DateTime.UtcNow;
    }

    public void RemoveApplicant(UserId applicant)
    {
        if (this.Status is not JobStatus.Active)
        {
            throw new InvalidOperationException($"Cannot remove applicant if job state is '{this.Status}'");
        }

        bool isApplicantAlreadyInPool = this._applicants.Contains(applicant);

        if (isApplicantAlreadyInPool)
        {
            // consider using domain event to notify other parts when this happened
            this._applicants.Remove(applicant);
        }
    }

    public void AddTag(TagId tag)
    {
        if (!this.Tags.AddTag(tag))
        {
            return;
        }

        this.UpdatedAt = DateTime.UtcNow;
    }

    public void RemoveTag(TagId tag)
    {
        if (this.Tags.RemoveTag(tag))
        {
            return;
        }

        this.UpdatedAt = DateTime.UtcNow;
    }

    public void AddTags(List<TagId> tags)
    {
        if (!this.Tags.AddTags(tags))
        {
            return;
        }

        this.UpdatedAt = DateTime.UtcNow;
    }

    public void RemoveTags(List<TagId> tags)
    {
        this.Tags.RemoveTags(tags);
        this.UpdatedAt = DateTime.UtcNow;
    }
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