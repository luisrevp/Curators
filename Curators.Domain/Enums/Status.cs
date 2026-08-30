namespace Curators.Domain.Enums;

public enum BookingStatus
{
    Pending,
    Confirmed,
    Rejected,
    Completed,
    Canceled,
    NoShow
}

public enum JobStatus
{
    Draft,
    Active,
    Paused,
    Closed,
    Archived
}

public enum MessageStatus
{
    Sent,
    Delivered,
    Read
}

public enum PaymentStatus
{
    Pending,
    Authorized,
    Completed,
    Failed,
    Refunded,
    PartiallyRefunded,
    Cancelled
}

public enum ServiceStatus
{
    Draft,
    Active,
    Paused,
    Archived
}