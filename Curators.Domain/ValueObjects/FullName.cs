namespace Curators.Domain.ValueObjects;

public sealed record FullName
{
    public string FirstName { get; }
    public string LastName { get; }
    public string Name => $"{this.FirstName} {this.LastName}";
    public string? Alias { get; init; }
    private FullName(string firstName, string lastName, string? alias = null)
    {
        FirstName = firstName;
        LastName = lastName;
        Alias = alias;
    }

    public static FullName Create(string firstName, string lastName, string? alias = null)
    {
        if (!IsInputValid(firstName)) 
            throw new ArgumentException($"{nameof(firstName)} is not valid!");
        if (!IsInputValid(lastName)) 
            throw new ArgumentException($"{nameof(lastName)} is not valid!");
        if (alias is not null && !IsInputValid(alias))
            throw new ArgumentException($"{nameof(alias)} is not valid!");

        return new FullName(firstName, lastName, alias);
    }

    private static bool IsInputValid(string field)
        => !string.IsNullOrWhiteSpace(field) && field.Length >= 2;
}
