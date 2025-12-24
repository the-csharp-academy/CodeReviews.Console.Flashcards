using Flashcards.TerrenceLGee.DTOs.StudySessionDTOs;
using Flashcards.TerrenceLGee.Models;

namespace Flashcards.TerrenceLGee.Mappings.StudySessionMappings;

public static class FromDto
{
    extension(CreateStudySessionDto dto)
    {
        public StudySession FromCreateStudySessionDto()
        {
            return new StudySession
            {
                StackId = dto.StackId,
                TotalQuestions = dto.TotalQuestions,
                Correct = dto.Correct,
                Incorrect = dto.Incorrect,
                Score = dto.Score,
                SessionDuration = dto.SessionDuration
            };
        }
    }
}