using Flashcards.Entities;

namespace Flashcards.Validation
{
    public static class FlashcardValidator
    {
        public static async Task<bool> ValidateFlashcard(Flashcard flashcard)
        {
            if (flashcard == null)
            {
                return false;
            }
            else if (string.IsNullOrWhiteSpace(flashcard.Front))
            {
                return false;
            }
            else if (string.IsNullOrWhiteSpace(flashcard.Back))
            {
                return false;
            }
            else
            {
                return true;
            }
        }
    }
}
