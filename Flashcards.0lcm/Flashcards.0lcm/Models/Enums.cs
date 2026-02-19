namespace Flashcards._0lcm.Models;

internal class Enums
{
    internal enum MainMenuOption
    {
        StackArea,
        FlashcardArea,
        StudyArea,
        Exit
    }

    internal enum StackAreaOption
    {
        ChangeName,
        Delete,
        Back
    }

    internal enum FlashcardAreaOption
    {
        UpdateNameAndValue,
        Delete,
        Back
    }

    internal enum StudyAreaOption
    {
        Back,
        Study,
        ViewPastSessions
    }

    internal enum StudyMenuOption
    {
        FlipCard
    }

    internal enum StudyMenuOption2
    {
        AssignFlashcardAsStudied,
        AssignFlashcardForReview
    }
}