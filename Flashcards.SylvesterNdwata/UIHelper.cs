using silvermax.FlashCards.Dtos;
using Spectre.Console;

namespace silvermax.FlashCards;

internal static class UIHelper
{
    public static void PressAnyKey()
    {
        AnsiConsole.MarkupLine("Press Any Key to continue...");
        Console.ReadKey();
    }

    public static int PromptId(string action, string entity)
    {
        var id = 0;
        AnsiConsole.Prompt(
            new TextPrompt<string>($"Please enter the Id of the {entity} you want to {action}: ")
            .Validate(input =>
            {
                if (!int.TryParse(input, out id))
                    return ValidationResult.Error($"Please enter a valid {entity} Id");
                return ValidationResult.Success();
            }));

        return id;
    }

    public static Table BuildFlashCardTable(IEnumerable<FlashCardResponseDto> cards)
    {
        var table = new Table { Border = TableBorder.Rounded };
        table.AddColumn("[yellow]Id[/]");
        table.AddColumn("[yellow]Front[/]");
        table.AddColumn("[yellow]Back[/]");

        int displayId = 1;
        foreach (var card in cards)
        {
            table.AddRow(
                (displayId++).ToString(),
                $"[purple]{card.Word}[/]",
                $"[purple]{card.Translation}[/]");
        }

        return table;
    }
}
