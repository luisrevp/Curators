namespace Curators.Domain.Enums;

public enum MessageStatus
{
    Sent,
    Delivered,
    Read
}

public enum BookingStatus
{
    Pending,
    Confirmed,
    Completed,
    Canceled,
    NoShow
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