using Flashcards.DTOs;
using Spectre.Console;

namespace Flashcards.Services
{
    internal static class FlashcardBuilder
    {
        internal static Table ShowFlashCardFront(FlashcardDTO flashcard, int totalCards)
        {
            AnsiConsole.Clear();

            var front = new Table()
                .RoundedBorder()
                .Width(20)
                .AddColumn("Question", col => col.Centered());


            front.AddRow($"{flashcard.Number}/{totalCards} \n\n");
            front.AddRow($"{flashcard.Front} \n\n").HeavyBorder();
            front.AddRow("");

            return front;
        }
        internal static Table ShowFlashcardBack(FlashcardDTO flashcard, bool correct)
        {
            AnsiConsole.Clear();

            var color = Color.Green;
            var result = "Correct Answer!";

            if (!correct)
            {
                color = Color.Red;
                result = "Incorrect Answer!";
            }

            var back = new Table()
                .Width(20)
                .RoundedBorder()
                .BorderColor(color)
                .AddColumn("Answer", col => col.Centered());

            back.AddEmptyRow();
            back.AddEmptyRow();
            back.AddRow(flashcard.Back);
            back.AddRow("");
            back.AddRow("");
            back.AddRow(result);

            return back;
        }
    }
}
