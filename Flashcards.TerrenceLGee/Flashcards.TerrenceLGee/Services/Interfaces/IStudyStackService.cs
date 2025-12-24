using Flashcards.TerrenceLGee.DTOs.StudyStackDTOs;

namespace Flashcards.TerrenceLGee.Services.Interfaces;

public interface IStudyStackService
{
    Task<int> AddStackAsync(CreateStudyStackDto dto);
    Task<int> UpdateStackAsync(UpdateStudyStackDto dto);
    Task<int> DeleteStackAsync(int stackId);
    Task<RetrievedStudyStackDto?> GetStackAsync(int stackId);
    Task<List<RetrievedStudyStackDto>> GetStacksAsync();
    Task<int> GetStackCountAsync();
    Task<Dictionary<int, string>> GetStackNameAndIdAsync();
    Task<string> GetStackNameAsync(int stackId);
}