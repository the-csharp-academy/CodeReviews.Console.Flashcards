using Flashcards.TerrenceLGee.Extensions;
using Flashcards.TerrenceLGee.FlashcardUI.Menus;
using Flashcards.TerrenceLGee.Models;
using Flashcards.TerrenceLGee.Services.Interfaces;
using Spectre.Console;

namespace Flashcards.TerrenceLGee.FlashcardUI.Helpers;

public class MenuHelpers
{
    private readonly IStudyStackService _stackService;

    public MenuHelpers(IStudyStackService stackService)
    {
        _stackService = stackService;
    }

    public async Task<StackMenu> GetStackMenuChoice()
    {
        var countOfStacks = await _stackService.GetStackCountAsync();

        var prompt = new SelectionPrompt<StackMenu>()
            .Title("[lightskyblue3_1]Please choose one of the following:[/]");
        prompt.AddChoice(StackMenu.AddStack);
        prompt.AddChoice(StackMenu.UpdateStack);
        prompt.AddChoice(StackMenu.DeleteStack);
        prompt.AddChoice(StackMenu.ViewStacks);
        prompt.AddChoice(StackMenu.ViewStack);

        if (countOfStacks > 0)
        {
            prompt.AddChoice(StackMenu.GoToFlashcardMenu);
            prompt.AddChoice(StackMenu.GoToStudySessionMenu);
        }

        prompt.AddChoice(StackMenu.Exit);

        prompt.UseConverter(choice => choice.GetDisplayName());
        
        var choice = AnsiConsole.Prompt(prompt);

        return choice;
    }

    public async Task<FlashcardMenu> GetFlashcardMenuChoice()
    {
        var countOfStacks = await _stackService.GetStackCountAsync();

        if (countOfStacks == 0)
        {
            InputHelpers
                .PressAnyKeyToContinueError("There are no stacks available to add a " +
                                            "flashcard to");
            return FlashcardMenu.Exit;
        }

        return AnsiConsole.Prompt(
            new SelectionPrompt<FlashcardMenu>()
                .Title("[lightskyblue3_1]Please choose one of the following[/]")
                .AddChoices(Enum.GetValues<FlashcardMenu>())
                .UseConverter(choice => choice.GetDisplayName()));
    }

    public async Task<StudySessionMenu> GetStudySessionMenu()
    {
        var countOfStacks = await _stackService.GetStackCountAsync();

        if (countOfStacks == 0)
        {
            InputHelpers
                .PressAnyKeyToContinueError("There are no stacks available to study");
            return StudySessionMenu.Exit;
        }

        return AnsiConsole.Prompt(
            new SelectionPrompt<StudySessionMenu>()
                .Title("[lightskyblue3_1]Please choose one of the following[/]")
                .AddChoices(Enum.GetValues<StudySessionMenu>())
                .UseConverter(choice => choice.GetDisplayName()));
    }

    public static Subject GetSubject()
    {
        return AnsiConsole.Prompt(
            new SelectionPrompt<Subject>()
                .Title("[lightskyblue3_1]Choose a subject to study for your stack[/]")
                .AddChoices(Enum.GetValues<Subject>())
                .UseConverter(choice => choice.GetDisplayName()));
    }
    
    public async Task<int> GetStackIdAsync(string message)
    {
        var stackInfo = await _stackService.GetStackNameAndIdAsync();
        var stackNames = new List<string>(stackInfo.Values);
        
        var stackName = AnsiConsole.Prompt(
            new SelectionPrompt<string>()
                .Title($"[palegreen1_1]{message}[/]")
                .AddChoices(stackNames));

        foreach (var id in stackInfo.Keys.Where(id => stackInfo[id] == stackName))
        {
            return id;
        }

        return -1;
    }
}