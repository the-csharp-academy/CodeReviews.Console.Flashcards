using Flashcards.TerrenceLGee.Models;

namespace Flashcards.TerrenceLGee.Data.Interfaces;

public interface IStudySessionRepository
{
    Task<int> AddStudySessionAsync(StudySession studySession);
    Task<StudySession?> GetStudySessionAsync(int stackId, int sessionId);
    Task<List<StudySession>> GetStudySessionsAsync(int stackId);
    Task<int> GetStudySessionCountAsync(int stackId);
}