using Dapper;
using silvermax.FlashCards.Controllers;
using silvermax.FlashCards.Dtos;
using silvermax.FlashCards.Models;
using Spectre.Console;

namespace silvermax.FlashCards;

internal class StudySession
{
    private readonly DatabaseHelper db = new();
    StacksController controller = new();
    public void Study()
    {
        AnsiConsole.MarkupLine("[blue]Welcome to the Study Session[/]");

        controller.GetAllStacks();

        var choosenStack = AnsiConsole.Ask<string>("Please write the name of the stack to want to study on: ");
        var numberOfQuestions = AnsiConsole.Ask<int>("What is the number of questions you want to do?: ");

        using (var connection = db.GetConnection())
        {
            int stackId = db.GetStackId(choosenStack);

            var allCards = connection.Query<FlashCardResponseDto>("SELECT * FROM Flashcards WHERE StackId = @StackId", new { StackId = stackId }).ToList();

            if (allCards.Count == 0)
            {
                AnsiConsole.MarkupLine("[red]No flashcards found in this stack.[/]");
                UIHelper.PressAnyKey();
                return;
            }

            var questions = allCards
                .OrderBy(_ => Random.Shared.Next())
                .Take(numberOfQuestions)
                .ToList();

            int score = 0;

            foreach (var card in questions)
            {
                var answer = AnsiConsole.Ask<string>($"[yellow]Translate:[/] [purple]{card.Word}[/] : ");

                if (answer.Trim().Equals(card.Translation, StringComparison.OrdinalIgnoreCase))
                {
                    AnsiConsole.MarkupLine("[green]Correct![/]");
                    score++;
                }
                else
                {
                    AnsiConsole.MarkupLine($"[red]Wrong! The correct answer was: {card.Translation}[/]");
                }
            }

            AnsiConsole.MarkupLine($"\n[blue]Session complete! Score: {score}/{questions.Count}[/]");
            InsertToDb(score);
            AnsiConsole.MarkupLine("[green]Session successfully saved in the database[/]");
            UIHelper.PressAnyKey();
        }
    }

    public void ViewStudySessionData()
    {
        using (var connection = db.GetConnection())
        {
            var table = new Table();
            table.Border = TableBorder.Rounded;

            table.AddColumn("[yellow]Id[/]");
            table.AddColumn("[yellow]Date[/]");
            table.AddColumn("[yellow]Score[/]");

            var sessions = connection.Query<Session>("SELECT * FROM Sessions").ToList();

            if (sessions.Count <= 0)
            {
                AnsiConsole.MarkupLine("[red]No rows found in the database[/]");
                UIHelper.PressAnyKey();
                return;
            }

            foreach (var session in sessions)
            {
                table.AddRow(
                    session.Id.ToString(),
                    $"[purple]{session.Date}[/]",
                    $"[purple]{session.Score}[/]"
                    );
            }

            AnsiConsole.Write(table);
        }

        UIHelper.PressAnyKey();
    }

    private void InsertToDb(int score)
    {
        var date = DateTime.UtcNow.Date;
        using (var connection = db.GetConnection())
        {
            string insertSql = "INSERT INTO Sessions (Date, Score) VALUES (@Date, @Score)";

            connection.Execute(insertSql, new { Date = date, Score = score });
        }
    }
}
