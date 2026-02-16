using Flashcards.m1chael888.Models;
using Spectre.Console;
using static Flashcards.m1chael888.Enums.ManageEnums;
using static Flashcards.m1chael888.Enums.EnumExtension;

namespace Flashcards.m1chael888.Views;

public interface ICardView
{
    void DisplayCardList(List<CardDto> cards, string stackName);
    ViewCardsOption DisplayCardMenu();
    string GetCardFront(string operation, bool error = false);
    string GetCardBack(string operation, bool error = false);
    CardDto DisplayCardPrompt(List<CardDto> cards, string title);
}
public class CardView : ICardView
{
    public ViewCardsOption DisplayCardMenu()
    {
        var choice = AnsiConsole.Prompt(
                        new SelectionPrompt<ViewCardsOption>()
                        .AddChoices(Enum.GetValues<ViewCardsOption>())
                        .UseConverter(x => GetDescription(x))
                        .HighlightStyle("lime")
                        .WrapAround()
                        );
        return choice;
    }

    public string GetCardFront(string operation, bool error = false)
    {
        Console.Clear();
        AnsiConsole.MarkupLine($"[lime]{operation} a card::[/]\n");
        return AnsiConsole.Ask<string>("[lime]Enter the front of the card (question)[/]");
    }

    public string GetCardBack(string operation, bool error = false)
    {
        Console.Clear();
        AnsiConsole.MarkupLine($"[lime]{operation} a card::[/]\n");
        return AnsiConsole.Ask<string>("[lime]Enter the back of the card (answer)[/]");
    }

    public void DisplayCardList(List<CardDto> cards, string stackName)
    {
        AnsiConsole.MarkupLine($"[lime]Cards in {stackName}::[/]\n");
        AnsiConsole.MarkupLine("[lime]Id[/]");
        foreach (CardDto card in cards)
        {
            string front = card.Front; string back = card.Back;
            AnsiConsole.MarkupLine($"{card.DisplayId}\t{CheckLength(front).PadRight(28)}\t{CheckLength(back).PadRight(28)}");
        }
        Console.WriteLine();
    }

    public CardDto DisplayCardPrompt(List<CardDto> cards, string title)
    {
        Console.Clear();
        return AnsiConsole.Prompt(
            new SelectionPrompt<CardDto>()
            .Title($"[lime]{title}[/]")
            .UseConverter(x => $"{x.DisplayId}\t{CheckLength(x.Front).PadRight(28)}\t{CheckLength(x.Back).PadRight(28)}")
            .AddChoices(cards)
            .HighlightStyle("lime")
            .WrapAround()
            );
    }

    string CheckLength(string myString)
    {
        if (myString.Length > 28) myString = myString.Substring(0, 25) + "...";
        return myString;
    }
}
