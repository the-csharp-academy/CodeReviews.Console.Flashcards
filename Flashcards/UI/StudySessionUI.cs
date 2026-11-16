using Flashcards.DTOs;
using Flashcards.Services;
using Spectre.Console;

namespace Flashcards.UI
{
    internal class StudySessionUI
    {
        private readonly FlashcardService _flashcardService;
        private readonly StudySessionService _studySessionService;
        public StudySessionUI(FlashcardService flashcardService, StudySessionService studySessionService)
        {
            _flashcardService = flashcardService;
            _studySessionService = studySessionService;
        }

        public async Task Show(StackDTO stack)
        {
            AnsiConsole.Clear();
            var flashcardDTOs = await _flashcardService.GetFlashcardsByStackAsync(stack);

            var totalCards = flashcardDTOs.Count;
            var correctAnswers = 0;
            var startTime = DateTime.Now;

            foreach (var f in flashcardDTOs)
            {
                AnsiConsole.Clear();

                AnsiConsole.Write(FlashcardBuilder.ShowFlashCardFront(f, totalCards));

                var answer = AnsiConsole.Prompt(new TextPrompt<String>("Please enter your answer")).ToLower().Replace(" ", "");

                if (answer != f.Back.ToLower().Replace(" ", ""))
                {
                    AnsiConsole.Write(FlashcardBuilder.ShowFlashcardBack(f, false));
                    AnsiConsole.MarkupLine("\nPress any key to continue...");
                    Console.ReadKey();
                    continue;
                }

                AnsiConsole.Write(FlashcardBuilder.ShowFlashcardBack(f, true));
                correctAnswers++;
                AnsiConsole.MarkupLine("\nPress any key to continue...");
                Console.ReadKey();
            }

            AnsiConsole.Clear();

            var endTime = DateTime.Now;

            var dto = await _studySessionService.CreateStudySessionDTOFromInputAsync(totalCards, stack.Id, startTime, endTime, correctAnswers);
            await _studySessionService.SaveStudySessionAsync(dto);


            AnsiConsole.MarkupLine($"[bold]You completed the study session." +
                $"\nYou achieved {dto.Percentage}% with {correctAnswers} correct answers out of a total of {totalCards}[/]");

            if (dto.Percentage < 80)
            {
                AnsiConsole.MarkupLine($"[red]\nYou should continue practicing this stack[/]");
            }
            else
            {
                AnsiConsole.MarkupLine($"[green]\nWell done! it seems you understand the stack quite well[/]");
            }

            AnsiConsole.MarkupLine("[blue]\nPress any key to continue...[/]");
            Console.ReadKey();
            AnsiConsole.Clear();

        }
    }
}
