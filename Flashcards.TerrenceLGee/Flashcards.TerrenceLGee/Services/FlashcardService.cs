using Flashcards.TerrenceLGee.Data.Interfaces;
using Flashcards.TerrenceLGee.DTOs.FlashcardDTOs;
using Flashcards.TerrenceLGee.Mappings.FlashcardMappings;
using Flashcards.TerrenceLGee.Services.Interfaces;

namespace Flashcards.TerrenceLGee.Services;

public class FlashcardService : IFlashcardService
{
    private readonly IFlashcardRepository _repository;

    public FlashcardService(IFlashcardRepository repository) => _repository = repository;


    public async Task<int> AddFlashcardAsync(CreateFlashcardDto dto)
    {
        return await _repository.AddFlashcardAsync(dto.FromCreateFlashcardDto());
    }

    public async Task<int> UpdateFlashcardAsync(UpdateFlashcardDto dto)
    {
        return await _repository.UpdateFlashcardAsync(dto.FromUpdateFlashcardDto());
    }

    public async Task<int> DeleteFlashcardAsync(int stackId, int position)
    {
        return await _repository.DeleteFlashcardAsync(stackId, position);
    }

    public async Task<RetrievedFlashcardDto?> GetFlashcardAsync(int stackId, int position)
    {
        var flashcard = await _repository.GetFlashcardAsync(stackId, position);
        return flashcard?.ToRetrievedFlashcardDto();
    }

    public async Task<List<RetrievedFlashcardDto>> GetFlashcardsAsync(int stackId)
    {
        var flashcards = await _repository.GetFlashcardsAsync(stackId);
        return flashcards
            .Select(f => f.ToRetrievedFlashcardDto())
            .ToList();
    }

    public async Task<int> GetFlashcardCountAsync(int stackId)
    {
        return await _repository.GetFlashcardCountAsync(stackId);
    }
}