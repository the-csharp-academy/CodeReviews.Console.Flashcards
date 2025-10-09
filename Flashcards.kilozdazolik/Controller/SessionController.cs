using Flashcards.kilozdazolik.Data;
using Flashcards.kilozdazolik.Models;
using Spectre.Console;

namespace Flashcards.kilozdazolik.Controller;

public class SessionController
{
    private Helper _helper = new();
    private StackRepository _stackRepository = new();
    private FlashcardRepository _flashcardRepository = new();
    private SessionRepository _sessionRepository = new();

    public void StartSession()
    {
        var stack = _helper.SelectStack(_stackRepository, "study");
        var flashCardList = _flashcardRepository.GetCardsByStack(stack.StackId);

        Random rnd = new();
        // Shuffle the list using the Fisher-Yates algorithm
        for (int i = flashCardList.Count - 1; i > 0; i--)
        {
            int j = rnd.Next(0, i + 1);
            var temp = flashCardList[i];
            flashCardList[i] = flashCardList[j];
            flashCardList[j] = temp;
        }

        int score = 0;

        for (int i = 0; i < flashCardList.Count; i++)
        {
            var currentCard = flashCardList[i];
            string answer = _helper.GetUserInputText($"Question ({i + 1}/{flashCardList.Count}): {currentCard.Front}");

            if (answer.Trim().Equals(currentCard.Back.Trim(), StringComparison.OrdinalIgnoreCase))
            {
                AnsiConsole.MarkupLine("[green]Correct![/]");
                score++;
            }
            else
            {
                AnsiConsole.MarkupLine($"[red]Incorrect![/] The answer was: [blue]{currentCard.Back}[/]");
            }
        }

        DateTime sessionDate = DateTime.Now;
        Session session = new() { StackId = stack.StackId, Date = sessionDate, Score = score };

        try
        {
            _sessionRepository.InsertSession(session);

            AnsiConsole.MarkupLine($"\n[bold yellow]--- SESSION COMPLETE ---[/]");
            AnsiConsole.MarkupLine($"You studied the '{stack.Name}' stack.");
            AnsiConsole.MarkupLine($"Your final score is: [bold green]{score} / {flashCardList.Count}[/]");
            AnsiConsole.MarkupLine("[grey]Results have been saved to your study history.[/]\n");
            AnsiConsole.MarkupLine("Press any key to continue...");
            Console.ReadKey();
        }
        catch (Exception ex)
        {
            AnsiConsole.MarkupLine($"[red]Error saving session history: {ex.Message}[/]");
        }
    }

    // TODO: Make the display DTO
    public void ViewAllSession()
    {
        var sessionList = _sessionRepository.GetStudySessions();
        var stackList = _stackRepository.GetAllStacks();

        if (sessionList.Any())
        {
            Dictionary<int, string> stackNameLookup = new();
            foreach (var stack in stackList)
            {
                stackNameLookup.Add(stack.StackId, stack.Name);
            }
            
            var table = new Table();
            table.Border(TableBorder.Rounded);

            table.AddColumn("[yellow]Date[/]");
            table.AddColumn("[yellow]Score[/]");
            table.AddColumn("[yellow]Stack[/]");

            foreach (var session in sessionList)
            {
                string correctName = stackNameLookup[session.StackId];
                
                table.AddRow(
                    $"[green]{session.Date}[/]",
                    $"[blue]{session.Score}[/]",
                    $"[yellow]{correctName}[/]"
                );
            }

            AnsiConsole.Write(table);
        }
        else
        {
            AnsiConsole.MarkupLine("There are no sessions available");
        }
    }
}