using Flashcards.TerrenceLGee.Data.Interfaces;
using Flashcards.TerrenceLGee.DTOs.StudyStackDTOs;
using Flashcards.TerrenceLGee.Mappings.StudyStackMappings;
using Flashcards.TerrenceLGee.Services.Interfaces;

namespace Flashcards.TerrenceLGee.Services;

public class StudyStackService : IStudyStackService
{
    private readonly IStudyStackRepository _repository;

    public StudyStackService(IStudyStackRepository repository) => _repository = repository;


    public async Task<int> AddStackAsync(CreateStudyStackDto dto)
    {
        return await _repository.AddStackAsync(dto.FromCreateStudyStackDto());
    }

    public async Task<int> UpdateStackAsync(UpdateStudyStackDto dto)
    {
        return await _repository.UpdateStackAsync(dto.FromUpdateStudyStackDto());
    }

    public async Task<int> DeleteStackAsync(int stackId)
    {
        return await _repository.DeleteStackAsync(stackId);
    }

    public async Task<RetrievedStudyStackDto?> GetStackAsync(int stackId)
    {
        var studyStack = await _repository.GetStackAsync(stackId);
        return studyStack?.ToRetrievedStudyStackDto();
    }

    public async Task<List<RetrievedStudyStackDto>> GetStacksAsync()
    {
        var studyStacks = await _repository.GetStacksAsync();
        return studyStacks
            .Select(s => s.ToRetrievedStudyStackDto())
            .ToList();
    }

    public async Task<int> GetStackCountAsync()
    {
        return await _repository.GetStackCountAsync();
    }

    public async Task<Dictionary<int, string>> GetStackNameAndIdAsync()
    {
        return await _repository.GetStackNameAndIdAsync();
    }

    public async Task<string> GetStackNameAsync(int stackId)
    {
        return await _repository.GetStackNameAsync(stackId);
    }
}