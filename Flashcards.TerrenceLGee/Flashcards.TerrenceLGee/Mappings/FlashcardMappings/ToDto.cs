using Flashcards.TerrenceLGee.DTOs.FlashcardDTOs;
using Flashcards.TerrenceLGee.Models;

namespace Flashcards.TerrenceLGee.Mappings.FlashcardMappings;

public static class ToDto
{
    extension(Flashcard flashcard)
    {
        public RetrievedFlashcardDto ToRetrievedFlashcardDto()
        {
            return new RetrievedFlashcardDto
            {
                Id = flashcard.Id,
                StackId = flashcard.StackId,
                Question = flashcard.Question,
                Answer = flashcard.Answer,
                Position = flashcard.Position
            };
        }
    }
}