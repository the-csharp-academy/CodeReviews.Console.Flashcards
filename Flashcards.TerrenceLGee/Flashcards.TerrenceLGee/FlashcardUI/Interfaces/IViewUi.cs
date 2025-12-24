namespace Flashcards.TerrenceLGee.FlashcardUI.Interfaces;

public interface IViewUi
{
    Task ViewStudyStacksAsync();
    Task ViewStudyStackAsync();
    Task ViewFlashcardsAsync();
    Task ViewFlashcardAsync();
    Task ViewStudySessionsAsync();
    Task ViewStudySessionAsync();
}