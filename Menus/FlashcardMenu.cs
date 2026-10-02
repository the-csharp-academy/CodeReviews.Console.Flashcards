using Flashcards.Controllers;
using Flashcards.DTOs;
using Spectre.Console;

namespace Flashcards.Menus
{
    public class FlashcardMenu
    {
        private readonly FlashcardController flashcardController;

        public FlashcardMenu(FlashcardController flashcardController)
        {
            this.flashcardController = flashcardController;
        }

        public void Run(CardStackDTO stackOwner)
        {
            while (true)
            {
                var flashactions = UI.GetActions(new List<string>
                {
                    "View flashcards",
                    "Add flashcard",
                    "Remove flashcards",
                    "Back"
                });

                if (flashactions == "View flashcards")
                    ViewFlashcards(stackOwner.Id);

                if (flashactions == "Add flashcard")
                    AddFlashcard(stackOwner.Id);

                if (flashactions == "Remove flashcards")
                    RemoveFlashcards(stackOwner.Id);

                if (flashactions == "Back")
                    return;
            }
        }

        private void ViewFlashcards(int stackId)
        {
            var flashCards = flashcardController.GetFlashcards(stackId).ToList();
            if (flashCards.Count == 0)
                UI.PrintMessage("No flashcards");
            else UI.PrintFlashcardTable(flashCards);
        }

        private void RemoveFlashcards(int stackId)
        {
            var flashCards = flashcardController.GetFlashcards(stackId).ToList();
            var selectedIds = UI.PrintFlashcardChoices(flashCards);
            foreach (var id in selectedIds)
                flashcardController.DeleteFlashcard(id);       
        }

        private void AddFlashcard(int stackId)
        {
            var front = UI.AskText("Enter front [blue]text[/] of flashcard:", 100);
            var back = UI.AskText("Enter back [blue]text[/] of flashcard:", 100);
            var flashcard = new CreateFlashcardDTO
            {
                CardStackId = stackId,
                Front = front,
                Back = back,
            };
            flashcardController.CreateFlashCard(flashcard);
        }
    }
}
