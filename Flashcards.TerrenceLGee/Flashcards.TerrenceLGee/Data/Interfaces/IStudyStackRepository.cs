using Flashcards.TerrenceLGee.Models;

namespace Flashcards.TerrenceLGee.Data.Interfaces;

public interface IStudyStackRepository
{
    Task<int> AddStackAsync(StudyStack stack);
    Task<int> UpdateStackAsync(StudyStack stack);
    Task<int> DeleteStackAsync(int stackId);
    Task<StudyStack?> GetStackAsync(int stackId);
    Task<List<StudyStack>> GetStacksAsync();
    Task<int> GetStackCountAsync();
    Task<Dictionary<int, string>> GetStackNameAndIdAsync();
    Task<string> GetStackNameAsync(int stackId);
}