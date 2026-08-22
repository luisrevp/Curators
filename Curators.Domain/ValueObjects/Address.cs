namespace Curators.Domain.ValueObjects;

public sealed record Address
{
    public string Street { get; }
    public string City { get; }
    public string State { get; }
    public string ZipCode { get; }
    public string Country { get; }

    private Address(string street, string city, string state, string zipCode, string country)
    {
        Street = street;
        City = city;
        State = state;
        ZipCode = zipCode;
        Country = country;
    }

    public static Address Create(string street, string city, string state, string zipCode, string country)
    {
        if (string.IsNullOrWhiteSpace(street)) throw new ArgumentException($"'{nameof(street)}' can't be null nor empty!");
        if (string.IsNullOrWhiteSpace(city)) throw new ArgumentException($"'{nameof(city)}' can't be null nor empty!");
        if (string.IsNullOrWhiteSpace(state)) throw new ArgumentException($"'{nameof(state)}' can't be null nor empty!");
        if (string.IsNullOrWhiteSpace(zipCode)) throw new ArgumentException($"'{nameof(zipCode)}' can't be null nor empty!");
        if (string.IsNullOrWhiteSpace(country)) throw new ArgumentException($"'{nameof(country)}' can't be null nor empty!");

        return new Address(street, city, state, zipCode, country);
    }
}
