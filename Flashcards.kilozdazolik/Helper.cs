using Spectre.Console;
using Flashcards.kilozdazolik.Data;
using Flashcards.kilozdazolik.Models;

namespace Flashcards.kilozdazolik;

public class Helper
{
    public bool ConfirmMessage(string message, string element, string color = "red")
    {
        var confirm = AnsiConsole.Confirm($"Are you sure you want to {message} [{color}]{element}[/]?");

        return confirm;
    }

    public string? GetUserInputText(string fieldName)
    {

        string text = AnsiConsole.Prompt(
            new TextPrompt<string>($"Please enter the {fieldName} ([yellow]Q[/] to cancel):")
        );

        if (text.Equals("Q", StringComparison.OrdinalIgnoreCase))
        {
            AnsiConsole.MarkupLine("[yellow]Edit cancelled, returning to main menu.[/]");
            return null;
        }
        
        return text;
    }
    
    public Stack SelectStack(StackRepository stackRepository, string action)
    {
        List<Stack> allStacks = stackRepository.GetAllStacks();

        if (allStacks.Count == 0)
        {
            AnsiConsole.MarkupLine($"[red]No stacks are available to {action}.[/]");
            Console.ReadKey();
        }

        var selectedStack = AnsiConsole.Prompt(
            new SelectionPrompt<Stack>()
                .Title($"Select a [cyan]STACK[/] to {action}:")
                .UseConverter(s => s.Name)
                .AddChoices(allStacks)
        );

        AnsiConsole.Clear();
        return selectedStack;
    }
}