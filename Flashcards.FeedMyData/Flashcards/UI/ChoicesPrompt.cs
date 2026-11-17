using Spectre.Console;

namespace Flashcards;

internal static class ChoicesPrompt
{
    internal static Stack Select(List<Stack> stacks, string title, string color)
    {
        var selection = AnsiConsole.Prompt(
                new SelectionPrompt<Stack>()
                .Title(title)
                .HighlightStyle(color)
                .UseConverter(stack => stack.Name)
                .AddChoices(stacks));

        return selection;
    }

    internal static Flashcard Select(List<Flashcard> flashcards, string title, string color)
    {
        var selection = AnsiConsole.Prompt(
                new SelectionPrompt<Flashcard>()
                .Title(title)
                .HighlightStyle(color)
                .UseConverter(f => f.Display)
                .AddChoices(flashcards));

        return selection;
    }

    internal static List<string> SelectMulti(string[] choices, string title, string color)
    {
        List<string> empty = new();

        var selection = AnsiConsole.Prompt(
                new MultiSelectionPrompt<string>()
                .InstructionsText($"[grey](Press <space> to toggle, <enter> to confirm, <escape> to cancel)[/]")
                .Title(title)
                .HighlightStyle(color)
                .AddCancelResult(empty)
                .AddChoices(choices));

        return selection;
    }

    internal static List<Stack> SelectMulti(List<Stack> stacks, string color)
    {
        List<Stack> empty = new();

        var selection = AnsiConsole.Prompt(
                new MultiSelectionPrompt<Stack>()
                .InstructionsText($"[grey](Press <space> to toggle, <enter> to confirm, <escape> to cancel)[/]")
                .HighlightStyle(color)
                .UseConverter(stack => stack.Name)
                .AddCancelResult(empty)
                .AddChoices(stacks));

        return selection;
    }

    internal static List<Flashcard> SelectMulti(List<Flashcard> flashcards, string color)
    {
        List<Flashcard> empty = new();

        var selection = AnsiConsole.Prompt(
                new MultiSelectionPrompt<Flashcard>()
                .InstructionsText($"[grey](Press <space> to toggle, <enter> to confirm, <escape> to cancel)[/]")
                .HighlightStyle(color)
                .UseConverter(f => f.Display)
                .AddCancelResult(empty)
                .AddChoices(flashcards));

        return selection;
    }

    internal static Enums.CrudChoice PromptCrudMenu(string itemType, params Enums.CrudChoice[] choices)
    {
        return AnsiConsole.Prompt(
            new SelectionPrompt<Enums.CrudChoice>()
                .Title($"[yellow]Manage {itemType}[/]")
                .HighlightStyle(Color.Yellow)
                .UseConverter(choice => choice.GetDisplayName())
                .AddChoices(choices));
    }

}