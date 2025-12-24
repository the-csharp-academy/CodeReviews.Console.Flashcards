using Flashcards.TerrenceLGee.DTOs.StudySessionDTOs;
using Flashcards.TerrenceLGee.Mappings.SessionFlashcardMappings;
using Flashcards.TerrenceLGee.Models;

namespace Flashcards.TerrenceLGee.Mappings.StudySessionMappings;

public static class ToDto
{
    extension(StudySession session)
    {
        public RetrievedStudySessionDto ToRetrievedStudySessionDto()
        {
            return new RetrievedStudySessionDto
            {
                Id = session.Id,
                StackId = session.StackId,
                TotalQuestions = session.TotalQuestions,
                Correct = session.Correct,
                Incorrect = session.Incorrect,
                Score = session.Score,
                SessionDuration = session.SessionDuration,
                SessionFlashcards = session.SessionFlashcards
                    .Select(sf => sf.ToSessionFlashcardDto())
                    .ToList()
            };
        }
    }
}