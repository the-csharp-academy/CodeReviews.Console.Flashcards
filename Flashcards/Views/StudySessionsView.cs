using Spectre.Console;

namespace CodeReviews.Console.Flashcards;

public sealed class StudySessionsView : IStudySessionsView
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
    public void DisplayMessage(string message) => AnsiConsole.MarkupLine(Markup.Escape(message));

    public void ShowFinalResult(int score, int totalQuestions)
    {
        AnsiConsole.Clear();

        double percentage = (double)score / totalQuestions * 100;

        AnsiConsole.MarkupLine(
            $"[bold green]Final score: " +
            $"{score}/{totalQuestions} " +
            $"({percentage:F0}%)[/]");

        WaitForInput();
    }

    public void ShowQuestion(string question, int currentNumber, int totalQuestions)
    {
        AnsiConsole.Clear();
        AnsiConsole.MarkupLine($"[grey]Question {currentNumber}/{totalQuestions}[/]");
        AnsiConsole.WriteLine();
        AnsiConsole.MarkupLine($"[bold cyan]Front:[/] {Markup.Escape(question)}");
    }

    public void WaitToRevealAnswer()
    {
        AnsiConsole.WriteLine();
        AnsiConsole.MarkupLine("[grey]Press any key to reveal the answer.[/]");
        AnsiConsole.Console.Input.ReadKey(intercept: true);
    }

    private static void WaitForInput()
    {
        AnsiConsole.MarkupLine("\n[grey]Press any key to continue.[/]");
        AnsiConsole.Console.Input.ReadKey(true);
    }

    public string TakeAnswerFromUser() => AnsiConsole.Prompt(new TextPrompt<string>("Your answer:").AllowEmpty());

    public void ShowAnswer(bool isCorrect, string answer)
    {
        AnsiConsole.WriteLine();

        if (isCorrect)
            AnsiConsole.MarkupLine("[bold green]Correct![/]");
        else
        {
            AnsiConsole.MarkupLine("[bold red]Incorrect.[/]");
            AnsiConsole.MarkupLine(
                $"[yellow]Correct answer:[/] " +
                $"{Markup.Escape(answer)}");
        }

        WaitForInput();
    }

    public void DisplayAllStudySessions(IReadOnlyList<StudySessionDTO> studies)
    {
        AnsiConsole.Clear();

        if (studies.Count == 0)
        {
            DisplayMessage("No records found.");
            WaitForInput();
            return;
        }

        var table = new Table()
            .AddColumn("Stack")
            .AddColumn("Score")
            .AddColumn("Total questions")
            .AddColumn("Mark")
            .AddColumn("Completed");

        foreach (var row in studies)
            table.AddRow(
                Markup.Escape(row.StackName),
                row.Score.ToString(),
                row.TotalQuestions.ToString(),
                FormatPercentage(row.Percentage),
                row.CompletedAt.ToString("yyyy-MM-dd HH:mm")
            );

        AnsiConsole.Write(table);
    }

    private static string FormatPercentage(double p) => $"{p}%";
}