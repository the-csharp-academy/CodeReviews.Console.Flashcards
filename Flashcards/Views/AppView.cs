using Spectre.Console;

namespace CodeReviews.Console.Flashcards;

public sealed class AppView : IAppView
{
    public MainMenuOption DisplayMainMenu() => AnsiConsole.Prompt(
        new SelectionPrompt<MainMenuOption>()
        .Title("[purple]Coding Tracker[/]")
        .AddChoices(Enum.GetValues<MainMenuOption>())
        .UseConverter(FormatMenuOption)
    );
    public void DisplayGoodbye() => AnsiConsole.MarkupLine("[yellow]Goodbye![/]");
    public void DisplayMessage(string message) => AnsiConsole.MarkupLine($"[green]{Markup.Escape(message)}[/]");
    private static string FormatMenuOption(MainMenuOption option)
    {
        return option switch
        {
            MainMenuOption.ManageStacks => "Manage stacks",
            MainMenuOption.ManageFlashcards => "Manage flashcards",
            MainMenuOption.Exit => "Exit",
            _ => option.ToString()
        };
    }
}