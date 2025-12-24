using Flashcards.TerrenceLGee.Data.Interfaces;
using Flashcards.TerrenceLGee.DTOs.SessionFlashcardDTOs;
using Flashcards.TerrenceLGee.Mappings.SessionFlashcardMappings;
using Flashcards.TerrenceLGee.Services.Interfaces;

namespace Flashcards.TerrenceLGee.Services;

public class SessionFlashcardService : ISessionFlashcardService
{
    private readonly ISessionFlashcardRepository _repository;

    public SessionFlashcardService(ISessionFlashcardRepository repository) => _repository = repository;


    public async Task<int> AddSessionFlashcardAsync(SessionFlashcardDto dto)
    {
        return await _repository.AddSessionFlashcardAsync(dto.FromSessionFlashcardDto());
    }
}