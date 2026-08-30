using Curators.Domain.Aggregates.UserAggregate;
using Curators.Domain.Enums;
using Curators.Domain.SeedWork;
using Curators.Domain.ValueObjects;
using System;
using System.Diagnostics;
using System.Net;
using System.Reflection;
using System.Text.RegularExpressions;
using System.Threading.Channels;

namespace Curators.Domain.Aggregates.PaymentAggregate;

public sealed class Payment : Entity<PaymentId>, IAggregateRoot
{
    public UserId PayerId { get; private set; }
    public UserId PayeeId { get; private set; }
    public Money Amount { get; private set; }
    public PaymentStatus Status { get; private set; }
    public PaymentMethod? PaymentMethod { get; private set; }
    public Guid? ExternalReference {  get; private set; } // if external provider was used
    public DateTime? PaidAt { get; private set; }
    public DateTime? RefundedAt { get; private set; }
    public DateTime CreatedAt { get; private set; }
    public DateTime UpdatedAt { get; private set; }

    private Payment(PaymentParameters payment)
    {
        this.PayerId = payment.PayerId;
        this.PayeeId = payment.PayeeId;
        this.Amount = payment.Amount;
        this.Status = payment.Status;
        this.PaymentMethod = payment.PaymentMethod;
        this.ExternalReference = payment.ExternalReference;
        this.PaidAt = payment.PaidAt;
        this.CreatedAt = DateTime.UtcNow;
        this.UpdatedAt = DateTime.UtcNow;
    }

    public static Payment AddNew(PaymentParameters parameters)
    {
        ArgumentNullException.ThrowIfNull(parameters);
        return new Payment(parameters);
    }

    public void Authorize(Money amount, PaymentMethod method)
    {
        if (!this.Status.Equals(PaymentStatus.Pending))
        {
            throw new InvalidOperationException($"Can't authorize payment. Current status is {this.Status}");
        }

        if (!Enum.IsDefined(method))
        {
            throw new InvalidOperationException($"Payment method {method} not allowed");
        }

        if (this.Amount != amount)
        {
            throw new InvalidOperationException("Payment amount and requested amount are different");
        }

        this.Status = PaymentStatus.Authorized;
        this.PaidAt = DateTime.UtcNow;
        this.UpdatedAt = DateTime.UtcNow;
    }

    // TO-DOs

    //Capture(Money amountToCapture)
    //Settles the authorized transaction and collects funds.Supports full or partial captures depending on fulfillment.
    //Invariants: Must be in an Authorized or PartiallyCaptured state.Amount captured cannot exceed the total authorized amount.
    
    //Fail(string reasonCode, string providerErrorMessage)
    //Marks the payment attempt as failed due to insufficient funds, processor declines, or fraud flags.
    //Invariants: Transition only allowed from active states like Pending, Processing, or Authorizing.
    
    //Cancel() / Void()
    //Cancels an open authorization before capture occurs, releasing held funds back to the user.
    //Invariants: Only valid while funds are in an Authorized or Pending state prior to settlement.
    
    //Refund(Money amountToRefund, string reason)
    //Returns captured funds to the payer (supports partial or full refunds).
    //Invariants: Payment must be in a Captured or PartiallyRefunded state.Cumulative refunded amount cannot exceed total captured amount.
    
    //RecordProviderReference(string gatewayTransactionId)
    //Attaches external payment processor identifiers(e.g., Stripe, MercadoPago reference tokens) once the gateway acknowledges the operation.
    //Invariants: Ensures idempotent linking between external transaction IDs and domain events.
    
    //MarkAsDisputed(string chargebackId, string reason)
    //Places the payment into a contested/chargeback state when a customer files a dispute through their issuing bank.
    //Invariants: Requires a settled/captured payment state.
}

public sealed record PaymentParameters(
    UserId PayerId,
    UserId PayeeId,
    Money Amount,
    PaymentStatus Status,
    PaymentMethod? PaymentMethod = null,
    Guid? ExternalReference = null,
    DateTime? PaidAt = null
);

public readonly record struct PaymentId(Guid Value)
{
    public static PaymentId Generate() => new PaymentId(Guid.NewGuid());
}

