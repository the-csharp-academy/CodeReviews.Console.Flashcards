using Flashcards.TerrenceLGee.DTOs.SessionFlashcardDTOs;
using Flashcards.TerrenceLGee.Models;

namespace Flashcards.TerrenceLGee.Mappings.SessionFlashcardMappings;

public static class FromDto
{
    extension(SessionFlashcardDto dto)
    {
        public SessionFlashcard FromSessionFlashcardDto()
        {
            return new SessionFlashcard
            {
                SessionId = dto.SessionId,
                FlashcardId = dto.FlashcardId,
                Question = dto.Question,
                Answer = dto.Answer,
                UserAnswer = dto.UserAnswer,
                IsCorrect = dto.IsCorrect,
                DisplayPosition = dto.DisplayPosition
            };
        }
    }
}