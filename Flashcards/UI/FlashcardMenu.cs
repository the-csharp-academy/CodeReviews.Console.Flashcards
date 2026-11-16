using Flashcards.DTOs;
using Flashcards.Services;
using Spectre.Console;

namespace Flashcards.UI
{
    internal class FlashcardMenu
    {
        private readonly StackService _stackService;
        private readonly FlashcardService _flashcardService;
        private List<FlashcardDTO> _flashcardList;
        private List<StackDTO> _stackList;
        public FlashcardMenu(StackService stackService, FlashcardService flashcardService)
        {
            _flashcardService = flashcardService;
            _stackService = stackService;
        }
        internal async Task Show()
        {
            bool show = true;

            while (show)
            {
                AnsiConsole.Clear();

                _flashcardList = await _flashcardService.GetAllFlashcardsAsync(null);
                _stackList = await _stackService.GetAllStacksDTOAsync(null);

                var table = TableBuilder.ShowFlashcardsTable(_flashcardList, _stackList);

                if (!_flashcardList.Any())
                {
                    AnsiConsole.MarkupLine("[red]No flashcards found in table[/]");
                }
                else
                {
                    AnsiConsole.Write(table);
                }

                var choice = AnsiConsole.Prompt(new SelectionPrompt<FlashcardMenuOptions>()
                    .Title("What do you want to do?")
                    .UseConverter(x => x switch
                    {
                        FlashcardMenuOptions.Create => "Create a new Flashcard",
                        FlashcardMenuOptions.Update => "Update an existing Flashcard",
                        FlashcardMenuOptions.Delete => "Delete a Flashcard",
                        FlashcardMenuOptions.Cancel => "Back"
                    })
                    .AddChoices(Enum.GetValues<FlashcardMenuOptions>()));

                switch (choice)
                {
                    case FlashcardMenuOptions.Create:
                        await CreateFlashcardAsync();
                        break;
                    case FlashcardMenuOptions.Update:
                        await UpdateFlashcardAsync();
                        break;
                    case FlashcardMenuOptions.Delete:
                        await DeleteFlashcardAsync();
                        break;
                    case FlashcardMenuOptions.Cancel:
                        show = false;
                        break;
                }
            }
        }
        internal async Task CreateFlashcardAsync()
        {
            var dto = await GetInputAsync();

            await _flashcardService.CreateFlashcardAsync(dto);
        }

        internal async Task UpdateFlashcardAsync()
        {
            var flashcard = AnsiConsole.Prompt(new SelectionPrompt<FlashcardDTO>()
                .Title("Please choose a flashcard to update:")
                .UseConverter(f =>
                {
                    var stack = _stackList.Where(s => s.Id == f.StackId).Single();
                    return $"{f.Number} - {f.Front} - {f.Back} - {stack.Name}";
                })
                .AddChoices(_flashcardList));

            var dto = await GetInputAsync();
            dto.Id = flashcard.Id;

            await _flashcardService.UpdateFlashcardAsync(dto);
        }

        internal async Task<FlashcardDTO> GetInputAsync()
        {
            var stackChoice = AnsiConsole.Prompt(new SelectionPrompt<StackDTO>()
                .Title("Please select a stack")
                .UseConverter(x =>
                {
                    return $"{x.Id} - {x.Name}";
                })
                .AddChoices(_stackList));

            var front = AnsiConsole.Prompt(new TextPrompt<string>("Please enter front flashcard text:")
                .AllowEmpty()
                .Validate(input =>
                {
                    if (string.IsNullOrWhiteSpace(input))
                    {
                        return ValidationResult.Error("[red]Flashcard front cannot be empty, please enter flashcard front details:[/]");
                    }

                    return ValidationResult.Success();
                }));

            var back = AnsiConsole.Prompt(new TextPrompt<string>("Please enter back flashcard text:")
                .AllowEmpty()
                .Validate(input =>
                {
                    if (string.IsNullOrWhiteSpace(input))
                    {
                        return ValidationResult.Error("[red]Flashcard back cannot be empty, please enter flashcard back details:[/]");
                    }
                    return ValidationResult.Success();
                }));

            return await _flashcardService.CreateFlashcardDTOFromInput(stackChoice, front, back);
        }

        internal async Task DeleteFlashcardAsync()
        {
            FlashcardDTO flashcardChoice = AnsiConsole.Prompt(new SelectionPrompt<FlashcardDTO>()
                .Title("Please select a flashcard to delete")
                .UseConverter(x =>
                {
                    var stack = _stackList.Where(s => s.Id == x.StackId).Single();
                    return $"{x.Number} - {x.Front} - {x.Back} - {stack.Name}";
                })
                .AddChoices(_flashcardList));

            await _flashcardService.DeleteFlashcardAsync(flashcardChoice);
        }
    }

    enum FlashcardMenuOptions
    {
        Create,
        Update,
        Delete,
        Cancel
    }
}
