using System.Text.RegularExpressions;

namespace Flashcards._0lcm.Models;

internal static class EnumExtensions
{
    internal static string ToDisplayString(this Enum value)
    {
        return Regex.Replace(
            value.ToString(),
            "([a-z])([A-Z])",
            "$1 $2"
        );
    }
}