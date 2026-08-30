using Curators.Domain.Enums;
using Curators.Domain.SeedWork;
using Curators.Domain.ValueObjects;
using Curators.Domain.Aggregates.PaymentAggregate;
using Curators.Domain.Aggregates.SerivceAggregate;
using Curators.Domain.Aggregates.UserAggregate;

namespace Curators.Domain.Aggregates.BookingAggregate;
public sealed class Booking : Entity<BookingId>, IAggregateRoot
{
    public UserId CustomerId { get; private set; }
    public UserId ProviderId { get; private set; }
    public ServiceId ServiceId { get; private set; }
    public DateRange ScheduledPeriod { get; private set; }
    public Pricing AgreedPricing { get; private set; }
    public CancellationPolicy AgreedCancellationPolicy { get; private set; }
    public BookingStatus Status { get; private set; }
    public PaymentId? PaymentId { get; private set; }
    public string? CancelllationReason { get; private set; }
    public CancellationActor? CancelledBy { get; private set;} 
    public CancellationRule? AppliedCancellationRule { get; private set; }
    public DateTime? CancelledAt { get; private set; }
    public DateTime CreatedAt { get; private set; }
    public DateTime UpdatedAt { get; private set; }

    private Booking(
        BookingParameters parameters
        ) : base(BookingId.Generate())
    {
        this.ProviderId = parameters.ProviderId;
        this.CustomerId = parameters.CustomerId;
        this.ServiceId = parameters.ServiceId;
        this.ScheduledPeriod = parameters.ScheduledPeriod;
        this.AgreedPricing = parameters.Pricing;
        this.AgreedCancellationPolicy = parameters.CancellationPolicy;
        this.Status = parameters.Status;
        this.PaymentId = parameters.PaymentId;
        this.CreatedAt = DateTime.UtcNow;
        this.UpdatedAt = DateTime.UtcNow;
    }

    public static Booking Request(BookingParameters bookingParameters)
    {
        ArgumentNullException.ThrowIfNull(bookingParameters);

        if (!Enum.IsDefined(bookingParameters.Status) && (bookingParameters.Status is not BookingStatus.Pending))
        {
            throw new ArgumentOutOfRangeException(
                paramName: nameof(bookingParameters.Status),
                actualValue: bookingParameters.Status,
                message: $"\"{bookingParameters.Status}\" is an invalid booking status value"
            );
        }

        if (bookingParameters.ScheduledPeriod.Start < DateTime.UtcNow)
        {
            throw new ArgumentOutOfRangeException(
                paramName: nameof(bookingParameters.ScheduledPeriod),
                message: $"Can't define a scheduled period prior to current time"
            );
        }

        return new Booking(bookingParameters);
    }

    public void Confirm()
    {
        if (this.Status is not BookingStatus.Pending)
        {
            throw new InvalidOperationException("Booking status must be in pending state");
        }

        if (this.ScheduledPeriod.Start > DateTime.UtcNow)
        {
            throw new InvalidOperationException("Scheduled time slot has already passed");
        }

        this.Status = BookingStatus.Confirmed;
        this.UpdatedAt = DateTime.UtcNow;
    }

    public void Reject(string reason)
    {
        if (this.Status is not BookingStatus.Pending)
        {
            throw new InvalidOperationException("Booking status must be in pending state in order to reject it");
        }

        ArgumentException.ThrowIfNullOrEmpty(reason);

        this.Status = BookingStatus.Rejected;
        this.UpdatedAt = DateTime.UtcNow;
    }

    public void Reschedule(DateRange newSchedule)
    {
        ArgumentNullException.ThrowIfNull(newSchedule);

        if (this.Status is (BookingStatus.Completed or BookingStatus.Canceled or BookingStatus.NoShow))
        {
            throw new InvalidOperationException($"Can't reschedule when status is \"{this.Status}\"");
        }

        if (newSchedule.Start.Day <= DateTime.UtcNow.Day)
        {
            throw new InvalidOperationException("Can't set a new schedule on the past");
        }
        // New period must be in the future and satisfy any lead-time policies (TBD)
        this.ScheduledPeriod = newSchedule;
        this.UpdatedAt = DateTime.UtcNow;
    }

    public void Cancel(CancellationActor actor, DateTime cancelledAt, string reason)
    {
        if (this.Status is (BookingStatus.Completed or BookingStatus.Canceled))
        {
            throw new InvalidOperationException($"Cannot cancel a booking request that is {this.Status}");
        }

        ArgumentException.ThrowIfNullOrEmpty(reason);

        var bookedDate = this.ScheduledPeriod.Start;
        var notice = bookedDate - cancelledAt;

        var cancellationPolicy = this.AgreedCancellationPolicy.GetApplicableRule(actor, notice);

        this.Status = BookingStatus.Canceled;
        this.CancelllationReason = reason;
        this.CancelledBy = actor;
        this.CancelledAt = cancelledAt;
        this.AppliedCancellationRule = cancellationPolicy;
        this.UpdatedAt = DateTime.UtcNow;
    }

    public void LinkPayment(PaymentId paymentId)
    {
        // Prevents overwriting an already attached payment ID
        // unless explicitly handling a retry scenario (TBD)
        this.PaymentId ??= paymentId;
        this.UpdatedAt = DateTime.UtcNow;
    }

    public void Complete()
    {
        if (this.Status is not BookingStatus.Confirmed)
        {
            throw new InvalidOperationException($"Can't complete a booking transaction if status is not {BookingStatus.Confirmed}");
        }

        if (this.ScheduledPeriod?.End > DateTime.UtcNow)
        {
            throw new InvalidOperationException("Scheduled period must have ended first");
        }

        this.Status = BookingStatus.Completed;
        this.UpdatedAt = DateTime.UtcNow;
    }


    public void MarkAsNoShow()
    {
        if (this.Status is not BookingStatus.Confirmed)
        {
            throw new InvalidOperationException($"Can't mark as No Show if status is '{BookingStatus.Confirmed}'");
        }

        if (this.ScheduledPeriod?.End > DateTime.UtcNow)
        {
            throw new InvalidOperationException("You can only mark as No Show with an elapsed scheduled period");
        }

        this.Status = BookingStatus.NoShow;
        this.UpdatedAt = DateTime.UtcNow;
    }
}

public sealed record BookingParameters(
    UserId CustomerId,
    UserId ProviderId,
    ServiceId ServiceId,
    DateRange ScheduledPeriod,
    Pricing Pricing,
    CancellationPolicy CancellationPolicy,
    BookingStatus Status = BookingStatus.Pending,
    PaymentId? PaymentId = null
);

public readonly record struct BookingId(Guid Value)
{
    public static BookingId Generate() => new BookingId(Guid.NewGuid());
}