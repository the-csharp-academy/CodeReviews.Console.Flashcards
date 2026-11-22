using Kerem.Flashcards.Domain.Entities;

namespace Kerem.Flashcards.Features ;

    public class FlashcardMapper
    {
        public static FlashcardDto MapToDto(Flashcard flashcard)
        {
            return new FlashcardDto()
            {
                Name = flashcard.Name,
            };
        }
    }