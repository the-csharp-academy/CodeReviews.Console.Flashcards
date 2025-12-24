using Flashcards.TerrenceLGee.DTOs.FlashcardDTOs;
using Flashcards.TerrenceLGee.FlashcardUI.Helpers;
using Flashcards.TerrenceLGee.FlashcardUI.Interfaces;
using Flashcards.TerrenceLGee.Mappings.FlashcardMappings;
using Flashcards.TerrenceLGee.Services.Interfaces;
using Spectre.Console;

namespace Flashcards.TerrenceLGee.FlashcardUI;

public class FlashcardUi : IFlashcardUi
{
    private readonly IFlashcardService _flashcardService;
    private readonly IStudyStackService _stackService;

    public FlashcardUi(
        IFlashcardService flashcardService,
        IStudyStackService stackService)
    {
        _flashcardService = flashcardService;
        _stackService = stackService;
    }


    public async Task AddFlashcardAsync()
    {
        AnsiConsole.WriteLine();
        var stackId = await new MenuHelpers(_stackService)
            .GetStackIdAsync("Please choose the name of the stack that you wish to add this flashcard to");

        if (stackId == -1)
        {
            InputHelpers
                .PressAnyKeyToContinueError("Stack not found");
            return;
        }

        var question = AnsiConsole.Ask<string>("[gold1]Enter the question (front) for the flashcard: [/]")
            .Trim();
        
        var answer = AnsiConsole.Ask<string>("[gold1]Enter the answer (back) for the flashcard: [/]")
            .Trim();

        var createFlashcard = await AnsiConsole.ConfirmAsync("[khaki1]\nAre you sure you wish to create this flashcard? [/]");

        if (!createFlashcard)
        {
            InputHelpers
                .PressAnyKeyToContinue($"Flashcard not added to stack {stackId}");
            return;
        }

        var position = (await _flashcardService.GetFlashcardCountAsync(stackId)) + 1;

        var flashcard = new CreateFlashcardDto
        {
            StackId = stackId,
            Question = question,
            Answer = answer,
            Position = position
        };

        if (await _flashcardService.AddFlashcardAsync(flashcard) != 1)
        {
            InputHelpers
                .PressAnyKeyToContinueError($"There was an error adding the flashcard to stack {stackId}");
            return;
        }

        InputHelpers.PressAnyKeyToContinue($"Flashcard successfully added to stack {stackId}");
    }

    public async Task UpdateFlashcardAsync()
    {
        var stackId = await new MenuHelpers(_stackService)
            .GetStackIdAsync("Please choose the name of the stack that this flashcard belongs to");

        if (stackId == -1)
        {
            InputHelpers
                .PressAnyKeyToContinueError("Stack not found");
            return;
        }
        
        var flashcards = await _flashcardService.GetFlashcardsAsync(stackId);
        var (isDisplayed, position) = InputHelpers
            .ShowPaginatedItems(flashcards, $"flashcards for stack {stackId}", ViewHelpers.DisplayFlashcards, 10, true,
                "the position of the flashcard that you wish to update or (0 to return to the previous menu)");

        if (!isDisplayed)
        {
            return;
        }

        switch (position)
        {
            case null:
                InputHelpers
                    .PressAnyKeyToContinueError($"Invalid position entered for flashcard in stack {stackId}");
                return;
            case 0:
                InputHelpers
                    .PressAnyKeyToContinue("Returning to the previous menu");
                return;
        }

        var flashcardToUpdate = await _flashcardService.GetFlashcardAsync(stackId, position.Value);

        if (flashcardToUpdate is null)
        {
            InputHelpers
                .PressAnyKeyToContinueError($"Error retrieving flashcard from stack {stackId}");
            return;
        }

        var updateQuestion = await AnsiConsole.ConfirmAsync("[khaki1]Do you wish to update the flashcard question (front)? [/]");
        string? updatedQuestion = null;

        if (updateQuestion)
        {
            updatedQuestion = AnsiConsole.Ask<string>("[gold1]Enter updated question: [/]")
                .Trim();
        }

        var updateAnswer = await AnsiConsole.ConfirmAsync("[khaki1]Do you wish to update the flashcard answer (back)? [/]");
        string? updatedAnswer = null;

        if (!updateQuestion && !updateAnswer)
        {
            InputHelpers
                .PressAnyKeyToContinueError("Nothing updated");
            return;
        }

        if (updateAnswer)
        {
            updatedAnswer = AnsiConsole.Ask<string>("[gold1]Enter updated answer: [/]")
                .Trim();
        }

        var updateFlashcard = await AnsiConsole
            .ConfirmAsync($"[khaki1]Are you sure you wish to update flashcard at position {flashcardToUpdate.Position}[/]");

        if (!updateFlashcard)
        {
            InputHelpers
                .PressAnyKeyToContinue($"Flashcard at position {position} not updated");
            return;
        }

        var updatedFlashcard = new UpdateFlashcardDto
        {
            Id = flashcardToUpdate.Id,
            StackId = stackId,
            Question = updatedQuestion ?? flashcardToUpdate.Question,
            Answer = updatedAnswer ?? flashcardToUpdate.Answer,
            Position = flashcardToUpdate.Position
        };

        if (await _flashcardService.UpdateFlashcardAsync(updatedFlashcard) != 1)
        {
            InputHelpers
                .PressAnyKeyToContinueError($"Unable to update flashcard at position {position} in stack {stackId}");
            return;
        }
        
        InputHelpers
            .PressAnyKeyToContinue($"Successfully updated flashcard at position {position} in stack {stackId}");
    }

    public async Task DeleteFlashcardAsync()
    {
        var stackId = await new MenuHelpers(_stackService)
            .GetStackIdAsync("Please choose the name of the stack that this flashcard belongs to");

        if (stackId == -1)
        {
            InputHelpers
                .PressAnyKeyToContinueError("Stack not found");
            return;
        }

        var flashcards = await _flashcardService.GetFlashcardsAsync(stackId);

        var (isDisplayed, position) = InputHelpers
            .ShowPaginatedItems(flashcards, $"flashcards for stack {stackId}", ViewHelpers.DisplayFlashcards, 10, true,
                "the position of the flashcard you wish to delete or (0 to return to the previous menu)");

        if (!isDisplayed)
        {
            return;
        }

        switch (position)
        {
            case null:
                InputHelpers
                    .PressAnyKeyToContinueError($"Invalid position entered for flashcard in stack {stackId}");
                return;
              case 0:
                  InputHelpers.PressAnyKeyToContinue("Returning to the previous menu");
                  return;
        }

        var wantToDeleteFlashcard = await AnsiConsole.ConfirmAsync($"[khaki1]Are you sure you wish to delete flashcard" +
                                                                   $" at position {position} in stack {stackId}[/]");

        if (!wantToDeleteFlashcard)
        {
            InputHelpers
                .PressAnyKeyToContinue($"Flashcard at position {position} in stack {stackId} not deleted");
            return;
        }

        if (await _flashcardService.DeleteFlashcardAsync(stackId, position.Value) != 1)
        {
            InputHelpers
                .PressAnyKeyToContinueError($"Error deleting flashcard at position {position} in stack {stackId}");
            return;
        } 
        
        flashcards = await _flashcardService.GetFlashcardsAsync(stackId);

        if (flashcards.Count == 0)
        {
            InputHelpers
                .PressAnyKeyToContinueError($"Flash card was deleted, but there was an unexpected error");
            return;
        }

        if (!await UpdateFlashcardPositionAsync(flashcards, position.Value))
        {
            InputHelpers
                .PressAnyKeyToContinueError($"Flash card was deleted, but there was an unexpected error updating " +
                                            $"the position of the subsequent flashcards");
            return;
        }
        
        InputHelpers.PressAnyKeyToContinue($"Flashcard at position {position} in stack {stackId} successfully deleted");
    }

    private async Task<bool> UpdateFlashcardPositionAsync(
        List<RetrievedFlashcardDto> flashcards, 
        int positionDeleted)
    {
        foreach (var flashcard in flashcards)
        {
            if (flashcard.Position > positionDeleted)
            {
                flashcard.Position--;
                var result = await _flashcardService
                    .UpdateFlashcardAsync(flashcard.FromRetrievedFlashcardDtoToUpdateFlashcardDto());
                if (result != 1) return false;
            }
        }

        return true;
    }

    
}