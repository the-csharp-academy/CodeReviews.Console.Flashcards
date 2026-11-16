using Flashcards.DTOs;
using Flashcards.Entities;

namespace Flashcards.Interfaces
{
    public interface IFlashcardRepository
    {
        Task<List<Flashcard>> GetAllFlashcardsAsync(int? stackId);
        Task UpdateFlashcardAsync(FlashcardDTO flashcard);
        Task DeleteFlashcardAsync(FlashcardDTO flashcard);
        Task CreateFlashcardAsync(FlashcardDTO flashcard);
    }
}
