namespace Flashcards.TerrenceLGee.FlashcardUI.Interfaces;

public interface IFlashcardUi
{
    Task AddFlashcardAsync();
    Task UpdateFlashcardAsync();
    Task DeleteFlashcardAsync();
}