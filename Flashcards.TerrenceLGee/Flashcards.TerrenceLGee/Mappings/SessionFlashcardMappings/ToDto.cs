using Flashcards.TerrenceLGee.DTOs.SessionFlashcardDTOs;
using Flashcards.TerrenceLGee.Models;

namespace Flashcards.TerrenceLGee.Mappings.SessionFlashcardMappings;

public static class ToDto
{
    extension(SessionFlashcard sessionFlashcard)
    {
        public SessionFlashcardDto ToSessionFlashcardDto()
        {
            return new SessionFlashcardDto
            {
                SessionId = sessionFlashcard.SessionId,
                FlashcardId = sessionFlashcard.FlashcardId,
                Question = sessionFlashcard.Question,
                Answer = sessionFlashcard.Answer,
                UserAnswer = sessionFlashcard.UserAnswer,
                IsCorrect = sessionFlashcard.IsCorrect,
                DisplayPosition = sessionFlashcard.DisplayPosition
            };
        }
    }
}