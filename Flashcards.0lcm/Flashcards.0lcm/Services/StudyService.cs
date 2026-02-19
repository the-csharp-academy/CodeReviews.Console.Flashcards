using Flashcards._0lcm.CRUDController;
using Flashcards._0lcm.DTOs;
using Flashcards._0lcm.Interfaces;
using Flashcards._0lcm.Models;

namespace Flashcards._0lcm.Services;

public class StudyService : IStudyService
{
    public List<FlashcardDto> RandomizeFlashcards(IFlashcardService flashcardService, StackDto stackDto)
    {
        var random = new Random();
        var flashcards = flashcardService.GetFlashcardDtosForStack(stackDto);

        var shuffledArray = flashcards.ToArray();
        random.Shuffle(shuffledArray);

        return shuffledArray.ToList();
    }

    public void LogStudySession(int studyCount, int stackId)
    {
        var studySession = new StudySession
        {
            StudyCount = studyCount,
            Date = DateTime.Today,
            StackId = stackId
        };

        LocalDbController.InsertSession(studySession);
    }

    public List<StudySessionDto> GetStudySessionDtosForStack(StackDto dto)
    {
        var sessions = LocalDbController.GetSessionsForStack(dto.StackId)
            .OrderBy(s => s.SessionId);

        var studySessionDtos = new List<StudySessionDto>();
        var displayId = 1;

        foreach (var session in sessions)
            studySessionDtos.Add(new StudySessionDto
            {
                SessionId = session.SessionId,
                DisplayId = displayId++,
                StudyCount = session.StudyCount,
                StackId = session.StackId
            });

        return studySessionDtos;
    }
}