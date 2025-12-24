using Flashcards.TerrenceLGee.DTOs.StudySessionDTOs;

namespace Flashcards.TerrenceLGee.Services.Interfaces;

public interface IStudySessionService
{
    Task<int> AddStudySessionAsync(CreateStudySessionDto dto);
    Task<RetrievedStudySessionDto?> GetStudySessionAsync(int stackId, int sessionId);
    Task<List<RetrievedStudySessionDto>> GetStudySessionsAsync(int stackId);
    Task<int> GetStudySessionCountAsync(int stackId);
}