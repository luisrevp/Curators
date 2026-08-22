using System.Text.RegularExpressions;
namespace Curators.Domain.ValueObjects;

// private constructor. Only the record itself is able to pass the value upon creation
// workflow is:
// Case 1 --> Email.Create("something@something.com") --> good! it creates an inmutable instance of Email
// Case 2 --> Email.Create("somethingggg") --> bad! but the factory method handles the exception

public sealed record Email
{
    public string Value { get; }
    private const string _emailPattern = "/^[a-zA-Z0-9._%+-]+@[a-zA-Z0-9.-]+\\.[a-zA-Z]{2,}$/";

    public static Email Create(string value)
    {
        if (!IsEmailValid(value))
        {
            throw new ArgumentException("Email is not valid!");
        }

        return new Email(value);
    }

    #region private constructor/methods
    private Email(string value)
    {
        this.Value = value;
    }
    private static bool IsEmailValid(string value)
        => !string.IsNullOrEmpty(value) && !string.IsNullOrWhiteSpace(value) && Regex.IsMatch(value, _emailPattern);
    
    #endregion
}
