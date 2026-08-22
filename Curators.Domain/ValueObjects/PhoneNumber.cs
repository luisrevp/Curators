namespace Curators.Domain.ValueObjects;

public sealed record PhoneNumber
{
    public string CountryCode { get; init; }
    public string NationalNumber { get; init; }
    public string? Extension { get; init; }

    public string E164_Format => $"{CountryCode}{NationalNumber}";

    private PhoneNumber() 
    {
        
    }
}
