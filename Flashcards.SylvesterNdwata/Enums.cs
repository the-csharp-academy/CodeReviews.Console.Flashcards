using System.Text.RegularExpressions;

namespace silvermax.FlashCards;

internal class Enums
{
    internal static string ToDisplayName(Enum value) =>
        Regex.Replace(value.ToString(), "(?<!^)([A-Z])", " $1");

    internal enum MenuOptions
    {
        Exit,
        ManageStacks,
        ManageFlashcards,
        Study,
        ViewStudySessionData,
    }

    internal enum ManageFlashCard
    {
        AddFlashcard,
        EditFlashcard,
        DeleteFlashcard,
        ViewAllFlashcards,
        Back,
    }

    internal enum ManageStack
    {
        AddStack,
        EditStack,
        DeleteStack,
        CreateFlashcardInStack,
        ShowAllFlashcardsInStack,
        Back,
    }
}
