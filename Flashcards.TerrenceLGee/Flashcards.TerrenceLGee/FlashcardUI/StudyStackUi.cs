using Flashcards.TerrenceLGee.DTOs.StudyStackDTOs;
using Flashcards.TerrenceLGee.Extensions;
using Flashcards.TerrenceLGee.FlashcardUI.Helpers;
using Flashcards.TerrenceLGee.FlashcardUI.Interfaces;
using Flashcards.TerrenceLGee.Models;
using Flashcards.TerrenceLGee.Services.Interfaces;
using Spectre.Console;

namespace Flashcards.TerrenceLGee.FlashcardUI;

public class StudyStackUi : IStudyStackUi
{
    private readonly IStudyStackService _stackService;

    public StudyStackUi(IStudyStackService stackService)
    {
        _stackService = stackService;
    }


    public async Task AddStudyStackAsync()
    {
        AnsiConsole.WriteLine();
        var subject = MenuHelpers.GetSubject();
        var name = AnsiConsole.Ask<string>("[honeydew2]Please enter a name for this stack: [/]")
            .Trim();

        var createStack = await AnsiConsole.ConfirmAsync("\n[violet]Are you sure you wish to create this stack? [/]");
        if (!createStack)
        {
            InputHelpers
                .PressAnyKeyToContinue("Stack not created");
            return;
        }

        var stack = new CreateStudyStackDto
        {
            Subject = subject,
            Name = name
        };

        if (await _stackService.AddStackAsync(stack) != 1)
        {
            InputHelpers
                .PressAnyKeyToContinueError("There was an error creating your stack");
            return;
        }
        
        InputHelpers.PressAnyKeyToContinue("Stack added successfully");
    }

    public async Task UpdateStudyStackAsync()
    {
        AnsiConsole.WriteLine();
        var stacks = await _stackService.GetStacksAsync();

        var (isDisplayed, id) = InputHelpers
            .ShowPaginatedItems(stacks, "study stacks", ViewHelpers.DisplayStudyStacks, 10, true, 
                "id of the stack you wish to update or (0 to return to the previous menu)");

        if (!isDisplayed)
        {
            return;
        }

        switch (id)
        {
            case null:
                InputHelpers
                    .PressAnyKeyToContinueError("The id you chose is invalid");
                return;
            case 0:
                InputHelpers
                    .PressAnyKeyToContinue("Returning to the previous menu");
                return;
        }

        var stackToUpdate = await _stackService.GetStackAsync(id.Value);

        if (stackToUpdate is null)
        {
            InputHelpers
                .PressAnyKeyToContinueError("There was an error retrieving the stack that you wish to update");
            return;
        }

        var updateSubject = await AnsiConsole
            .ConfirmAsync(
                $"[violet]Do you wish to update the stack's subject? Currently it is:[/]" +
                $" [lightgoldenrod1]{stackToUpdate.Subject.GetDisplayName()}[/] ");

        Subject? updatedSubject = null;

        if (updateSubject)
        {
            updatedSubject = MenuHelpers.GetSubject();
        }

        var updateName = await AnsiConsole
            .ConfirmAsync(
            $"[violet]Do you wish to update the stack's name? Currently it is:[/]" +
            $" [lightgoldenrod1]{stackToUpdate.Name}[/] ");

        if (!updateSubject && !updateName)
        {
            InputHelpers
                .PressAnyKeyToContinueError("Nothing being updated");
            return;
        }

        var updatedName = string.Empty;
        if (updateName)
        {
            updatedName = AnsiConsole.Ask<string>("[honeydew2]Enter updated stack name: [/]");
        }

        var updateStack = await AnsiConsole.ConfirmAsync("[violet]Are you sure you want to update this stack? [/]");

        if (!updateStack)
        {
            InputHelpers
                .PressAnyKeyToContinue("Nothing being updated");
            return;
        }

        var updatedStack = new UpdateStudyStackDto
        {
            Id = stackToUpdate.Id,
            Subject = updatedSubject ?? stackToUpdate.Subject,
            Name = updateName ? updatedName : stackToUpdate.Name
        };

        if (await _stackService.UpdateStackAsync(updatedStack) != 1)
        {
            InputHelpers
                .PressAnyKeyToContinueError($"There was an error updating stack {stackToUpdate.Id}");
            return;
        }
        
        InputHelpers
            .PressAnyKeyToContinue($"Stack {stackToUpdate.Id} successfully updated");
    }

    public async Task DeleteStudyStackAsync()
    {
        AnsiConsole.WriteLine();
        var stacks = await _stackService.GetStacksAsync();

        var (isDisplayed, id) = InputHelpers
            .ShowPaginatedItems(stacks, "study stacks", ViewHelpers.DisplayStudyStacks, 10, true, 
                "the id of the stack you wish to delete or (0 to return to the previous menu)");

        if (!isDisplayed)
        {
            return;
        }

        switch (id)
        {
            case null:
                InputHelpers
                    .PressAnyKeyToContinueError("The id you chose is invalid");
                return;
            case 0:
                InputHelpers
                    .PressAnyKeyToContinue("Returning to the previous menu");
                return;
        }

        var deleteStack = await AnsiConsole.ConfirmAsync($"[violet]Are you you sure you wish to delete this stack? [/]");

        if (!deleteStack)
        {
            InputHelpers
                .PressAnyKeyToContinue("Nothing deleted");
            return;
        }

        if (await _stackService.DeleteStackAsync(id.Value) != 1)
        {
            InputHelpers
                .PressAnyKeyToContinueError($"There was an error deleting stack {id.Value}");
            return;
        }
        
        InputHelpers.PressAnyKeyToContinue($"Stack {id.Value} successfully deleted");
    }
}