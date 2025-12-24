using Flashcards.TerrenceLGee.Models;

namespace Flashcards.TerrenceLGee.Data.Interfaces;

public interface IFlashcardRepository
{
    Task<int> AddFlashcardAsync(Flashcard flashcard);
    Task<int> UpdateFlashcardAsync(Flashcard flashcard);
    Task<int> DeleteFlashcardAsync(int stackId, int position);
    Task<Flashcard?> GetFlashcardAsync(int stackId, int position);
    Task<List<Flashcard>> GetFlashcardsAsync(int stackId);
    Task<int> GetFlashcardCountAsync(int stackId);
    
}