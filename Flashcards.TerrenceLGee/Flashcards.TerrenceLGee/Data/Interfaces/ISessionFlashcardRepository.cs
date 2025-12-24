using Flashcards.TerrenceLGee.Models;

namespace Flashcards.TerrenceLGee.Data.Interfaces;

public interface ISessionFlashcardRepository
{
    Task<int> AddSessionFlashcardAsync(SessionFlashcard sessionFlashcard);
}