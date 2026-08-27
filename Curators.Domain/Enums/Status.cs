namespace Curators.Domain.Enums;

public enum BookingStatus
{
    Pending,
    Confirmed,
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
    Unpaid,
    Authorized,
    Paid,
    Failed,
    Refunded
}

public enum ServiceStatus
{
    Draft,
    Active,
    Paused,
    Archived
}