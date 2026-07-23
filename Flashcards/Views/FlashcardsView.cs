namespace CodeReviews.Console.Flashcards;

using System.Collections.Generic;
using Spectre.Console;

public sealed class FlashcardsView : IFlashcardsView
{
    public void DisplayError(string message)
    {
        AnsiConsole.MarkupLine($"[red]{Markup.Escape(message)}[/]");
        WaitForInput();
    }

    public void DisplayFlashcards(IReadOnlyList<FlashcardDTO> cards)
    {
        AnsiConsole.Clear();

        if (cards.Count == 0)
        {
            DisplayMessage("No records found.");
            WaitForInput();
            return;
        }

        var table = new Table()
            .AddColumn("No.")
            .AddColumn("Question")
            .AddColumn("Answer");

        foreach (var row in cards)
            table.AddRow(
                row.DisplayId.ToString(),
                Markup.Escape(row.Question),
                Markup.Escape(row.Answer));

        AnsiConsole.Write(table);
    }

    public void DisplayMessage(string message) => AnsiConsole.MarkupLine($"[green]{Markup.Escape(message)}[/]");

    public FlashcardsOption ShowFlashcardsOption()
        => AnsiConsole.Prompt(
            new SelectionPrompt<FlashcardsOption>()
                .Title("\nWhat's next?")
                .AddChoices(Enum.GetValues<FlashcardsOption>()));
    public void WaitForInput()
    {
        AnsiConsole.MarkupLine("\n[grey]Press any key to continue.[/]");
        AnsiConsole.Console.Input.ReadKey(true);
    }

    public string SelectStack()
        => AnsiConsole.Prompt(
            new TextPrompt<string>("Enter the stack name:")
            .Validate(name =>
                string.IsNullOrWhiteSpace(name) ?
                ValidationResult.Error("[red]Stack name cannot be empty.[/]") :
                ValidationResult.Success())
        ).Trim();

    public (string question, string answer) AskFlashcardContent()
    {
        string question = AnsiConsole.Prompt(
            new TextPrompt<string>("Enter the question:")
                .Validate(value =>
                    string.IsNullOrWhiteSpace(value)
                        ? ValidationResult.Error(
                            "[red]Question cannot be empty.[/]")
                        : ValidationResult.Success()))
        .Trim();

        string answer = AnsiConsole.Prompt(
                new TextPrompt<string>("Enter the answer:")
                    .Validate(value =>
                        string.IsNullOrWhiteSpace(value)
                            ? ValidationResult.Error(
                                "[red]Answer cannot be empty.[/]")
                            : ValidationResult.Success()))
            .Trim();

        return (question, answer);
    }

    public int AskFlashcardIndex(int maxIndex)
        => AnsiConsole.Prompt(
            new TextPrompt<int>("Enter the flashcard number to delete:")
                .Validate(value => value < 1 || value > maxIndex
                    ? ValidationResult.Error($"[red]Please enter a number between 1 and {maxIndex}.[/]")
                    : ValidationResult.Success()));
}