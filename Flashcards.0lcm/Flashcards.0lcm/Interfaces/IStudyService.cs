using Flashcards._0lcm.DTOs;

namespace Flashcards._0lcm.Interfaces;

public interface IStudyService
{
    List<FlashcardDto> RandomizeFlashcards(IFlashcardService flashcardService, StackDto stackDto);
    void LogStudySession(int studyCount, int stackId);
    List<StudySessionDto> GetStudySessionDtosForStack(StackDto dto);
}