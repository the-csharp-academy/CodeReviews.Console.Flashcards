using Flashcards.TerrenceLGee.DTOs.SessionFlashcardDTOs;

namespace Flashcards.TerrenceLGee.Services.Interfaces;

public interface ISessionFlashcardService
{
    Task<int> AddSessionFlashcardAsync(SessionFlashcardDto dto);
}