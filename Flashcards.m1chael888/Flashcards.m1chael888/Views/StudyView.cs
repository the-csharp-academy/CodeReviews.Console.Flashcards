using Spectre.Console;
using static Flashcards.m1chael888.Enums.StudyViewEnums;
using static Flashcards.m1chael888.Enums.EnumExtension;
using Flashcards.m1chael888.Models;

namespace Flashcards.m1chael888.Views;

public interface IStudyView
{
    StudyMenuOption ShowMenu();
    void ShowFront(CardDto card);
    CardResult ShowBack(CardDto card);
    void ShowEnd(string score, int count);
    void ShowSessionHistory(List<SessionModel> sessions);
}
public class StudyView : IStudyView
{
    public StudyMenuOption ShowMenu()
    {
        var choice = AnsiConsole.Prompt(
            new SelectionPrompt<StudyMenuOption>()
                .Title("[lime]Study Menu::[/]")
                .AddChoices(Enum.GetValues<StudyMenuOption>())
                .UseConverter(x => GetDescription(x))
                .HighlightStyle("lime")
                .WrapAround()
                );
        return choice;
    }

    public void ShowFront(CardDto card)
    {
        AnsiConsole.MarkupLine($"[lime]Card {card.DisplayId}[/]\n");
        AnsiConsole.MarkupLine($"[lime]Question:[/] {card.Front}");
        ReturnStatus("Press any key to show the answer");
    }

    public CardResult ShowBack(CardDto card)
    {
        Console.Clear();
        AnsiConsole.MarkupLine($"[lime]Card {card.DisplayId}[/]\n");
        AnsiConsole.MarkupLine($"[lime]Answer:[/] {card.Back}\n");

        var result = AnsiConsole.Prompt(
            new SelectionPrompt<CardResult>()
                .AddChoices(Enum.GetValues<CardResult>())
                .UseConverter(x => GetDescription(x))
                .HighlightStyle("lime")
                .WrapAround());
        return result;
    }

    public void ShowEnd(string score, int count)
    {
        Console.Clear();
        switch (count)
        {
            case (>0):
                AnsiConsole.MarkupLine("[lime]Session saved..[/]\n");
                AnsiConsole.MarkupLine($"[lime]Score:[/] {score}");
                break;
            case (0):
                AnsiConsole.MarkupLine("[lime]Session canceled, not saved[/]\n");
                break;
        }
        ReturnStatus("Press any key to return");
    }

    public void ShowSessionHistory(List<SessionModel> sessions)
    {
        AnsiConsole.MarkupLine("[lime]Session History::[/]\n");
        AnsiConsole.MarkupLine("[lime]Id\tDate\t\tScore\tStack studied[/]");

        foreach (var session in sessions)
        {
            AnsiConsole.MarkupLine($"{session.SessionId}\t{session.Date}\t{session.Score}\t{session.StackId}");
        }
        ReturnStatus("Press any key to return to menu");
    }

    private void ReturnStatus(string msg)
    {
        AnsiConsole.Status()
            .Spinner(Spinner.Known.Point)
            .SpinnerStyle("white")
            .Start(msg, x =>
            {
                Console.ReadKey();
            });
    }
}
