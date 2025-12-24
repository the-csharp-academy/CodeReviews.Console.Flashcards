using Flashcards.TerrenceLGee.Data.Interfaces;
using Flashcards.TerrenceLGee.DTOs.StudySessionDTOs;
using Flashcards.TerrenceLGee.Mappings.StudySessionMappings;
using Flashcards.TerrenceLGee.Services.Interfaces;

namespace Flashcards.TerrenceLGee.Services;

public class StudySessionService : IStudySessionService
{
    private readonly IStudySessionRepository _repository;

    public StudySessionService(IStudySessionRepository repository) => _repository = repository;


    public async Task<int> AddStudySessionAsync(CreateStudySessionDto dto)
    {
        return await _repository.AddStudySessionAsync(dto.FromCreateStudySessionDto());
    }

    public async Task<RetrievedStudySessionDto?> GetStudySessionAsync(int stackId, int sessionId)
    {
        var session = await _repository.GetStudySessionAsync(stackId, sessionId);
        return session?.ToRetrievedStudySessionDto();
    }

    public async Task<List<RetrievedStudySessionDto>> GetStudySessionsAsync(int stackId)
    {
        var sessions = await _repository.GetStudySessionsAsync(stackId);
        return sessions
            .Select(s => s.ToRetrievedStudySessionDto())
            .ToList();
    }

    public async Task<int> GetStudySessionCountAsync(int stackId)
    {
        return await _repository.GetStudySessionCountAsync(stackId);
    }
}