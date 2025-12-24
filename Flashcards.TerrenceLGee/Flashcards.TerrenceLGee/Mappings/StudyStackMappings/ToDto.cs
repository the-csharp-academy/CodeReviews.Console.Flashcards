using Flashcards.TerrenceLGee.DTOs.FlashcardDTOs;
using Flashcards.TerrenceLGee.DTOs.StudySessionDTOs;
using Flashcards.TerrenceLGee.DTOs.StudyStackDTOs;
using Flashcards.TerrenceLGee.Mappings.FlashcardMappings;
using Flashcards.TerrenceLGee.Mappings.StudySessionMappings;
using Flashcards.TerrenceLGee.Models;

namespace Flashcards.TerrenceLGee.Mappings.StudyStackMappings;

public static class ToDto
{
    extension(StudyStack stack)
    {
        public RetrievedStudyStackDto ToRetrievedStudyStackDto()
        {
            return new RetrievedStudyStackDto
            {
                Id = stack.Id,
                Subject = stack.Subject,
                Name = stack.Name,
                Flashcards = stack.GetFlashcards(),
                StudySessions = stack.GetStudySessions()
            };
        }

        private List<RetrievedFlashcardDto> GetFlashcards()
        {
            return stack.Flashcards
                .Select(f => f.ToRetrievedFlashcardDto())
                .ToList();
        }

        private List<RetrievedStudySessionDto> GetStudySessions()
        {
            return stack.StudySessions
                .Select(s => s.ToRetrievedStudySessionDto())
                .ToList();
        }
    }
}