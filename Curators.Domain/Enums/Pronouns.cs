using System.ComponentModel;
namespace Curators.Domain.Enums;

public enum Pronouns
{
    [Description("He / Him / His")]
    HeHim = 0,
    [Description("She / Her / Hers")]
    SheHer = 1,
    [Description("They / Them / Theirs")]
    TheyThem = 2,
    [Description("It / Its")]
    ItIts = 3
}
