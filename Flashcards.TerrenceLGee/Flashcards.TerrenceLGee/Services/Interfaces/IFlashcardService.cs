using Flashcards.TerrenceLGee.DTOs.FlashcardDTOs;

namespace Flashcards.TerrenceLGee.Services.Interfaces;

public interface IFlashcardService
{
    Task<int> AddFlashcardAsync(CreateFlashcardDto dto);
    Task<int> UpdateFlashcardAsync(UpdateFlashcardDto dto);
    Task<int> DeleteFlashcardAsync(int stackId, int position);
    Task<RetrievedFlashcardDto?> GetFlashcardAsync(int stackId, int position);
    Task<List<RetrievedFlashcardDto>> GetFlashcardsAsync(int stackId);
    Task<int> GetFlashcardCountAsync(int stackId);
}