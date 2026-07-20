namespace CodeReviews.Console.Flashcards;

using Spectre.Console;

public sealed class StacksView : IStacksView
{
    public string AskForStackName()
        => AnsiConsole.Prompt(
            new TextPrompt<string>("Enter the stack name:")
            .Validate(name =>
                string.IsNullOrWhiteSpace(name) ?
                ValidationResult.Error("[red]Stack name cannot be empty.[/]") :
                ValidationResult.Success())
        ).Trim();

    public void DisplayError(string message)
    {
        AnsiConsole.MarkupLine($"[red]{Markup.Escape(message)}[/]");
        WaitForInput();
    }

    public void DisplayMessage(string message) => AnsiConsole.MarkupLine($"[green]{Markup.Escape(message)}[/]");
    public void DisplayStacks(IReadOnlyList<CardStackDTO> stacks)
    {
        AnsiConsole.Clear();

        if (stacks.Count == 0)
        {
            DisplayMessage("No records found.");
            WaitForInput();
            return;
        }

        var table = new Table()
            .AddColumn("Name")
            .AddColumn("Amount of cards");

        foreach (var row in stacks)
            table.AddRow(Markup.Escape(row.Name), row.CardCount.ToString());

        AnsiConsole.Write(table);
    }

    public StacksOption ShowStacksOption()
        => AnsiConsole.Prompt(
            new SelectionPrompt<StacksOption>()
                .Title("\nWhat's next?")
                .AddChoices(Enum.GetValues<StacksOption>()));

    private static void WaitForInput()
    {
        AnsiConsole.MarkupLine("\n[grey]Press any key to continue.[/]");
        AnsiConsole.Console.Input.ReadKey(true);
    }
}