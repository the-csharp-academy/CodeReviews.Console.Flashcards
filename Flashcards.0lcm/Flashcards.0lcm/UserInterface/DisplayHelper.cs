using Flashcards._0lcm.Models;
using Spectre.Console;
using Spectre.Console.Rendering;

namespace Flashcards._0lcm.UserInterface;

internal class DisplayHelper
{
    //------- Colors -------
    internal const string White = "#f1f1f1";
    private const string Grey = "#8c8e8f";
    private const string Green = "#32aa3b";
    private const string Red = "#cd2d2d";
    internal const string Yellow = "#e2b929";
    private const string Error = "#870c00";

    //------- Basic Outputs -------
    internal static void DisplayMessage(string message, bool writeLine = true)
    {
        if (writeLine)
            AnsiConsole.MarkupLine($"[{White}]{message}[/]");
        else
            AnsiConsole.Markup($"[{White}]{message}[/]");
    }

    internal static void DisplayRows(List<IRenderable> rows, bool writeLine = true)
    {
        var rowsLayout = new Rows(rows);
        AnsiConsole.Write(rowsLayout);
    }

    internal static void DisplayInfo(string info, bool writeLine = true)
    {
        if (writeLine)
            AnsiConsole.MarkupLine($"[{Grey}]{info}[/]");
        else
            AnsiConsole.Markup($"[{Grey}]{info}[/]");
    }

    internal static void DisplaySuccess(string message, bool writeLine = true)
    {
        if (writeLine)
            AnsiConsole.MarkupLine($"[{Green}]{message}[/]");
        else
            AnsiConsole.Markup($"[{Green}]{message}[/]");
    }

    internal static void DisplayUrgent(string message, bool writeLine = true)
    {
        if (writeLine)
            AnsiConsole.MarkupLine($"[{Red}]{message}[/]");
        else
            AnsiConsole.Markup($"[{Red}]{message}[/]");
    }

    internal static void DisplayWarning(string message, bool writeLine = true)
    {
        if (writeLine)
            AnsiConsole.MarkupLine($"[{Yellow}]{message}[/]");
        else
            AnsiConsole.Markup($"[{Yellow}]{message}[/]");
    }

    internal static void DisplayError(string message, bool writeLine = true)
    {
        if (writeLine)
            AnsiConsole.MarkupLine($"[{Error}]{message}[/]");
        else
            AnsiConsole.Markup($"[{Error}]{message}[/]");
    }

    //------- Menus & Prompts -------
    internal static T DisplayMenu<T>(string? title = null) where T : Enum
    {
        var menuChoice = AnsiConsole.Prompt(
            new SelectionPrompt<T>()
                .Title(title ?? "Please Select An Option:")
                .HighlightStyle(Style.Parse("darkviolet"))
                .AddChoices(Enum.GetValues(typeof(T)).Cast<T>())
                .UseConverter(e => e.ToDisplayString())
        );


        return menuChoice;
    }

    internal static string DisplayPrompt(List<string> choiceList, string? title = null)
    {
        var choice = AnsiConsole.Prompt(
            new SelectionPrompt<string>()
                .Title(title ?? "Please Select An Option:")
                .HighlightStyle(Style.Parse("darkviolet"))
                .AddChoices(choiceList));

        return choice;
    }

    internal static List<string> DisplayMultiPrompt(List<string> choiceList, string? title = null,
        bool requireChoice = true)
    {
        var prompt = new MultiSelectionPrompt<string>()
            .Title(title ?? "Please Select An Option:")
            .HighlightStyle(Style.Parse("darkviolet"))
            .InstructionsText($"[{Grey}]Press[/] [{White}]<Space>[/] to Toggle, and [{White}]<enter>[/] to Confirm")
            .AddChoices(choiceList);

        if (requireChoice)
            prompt.Required();
        else
            prompt.NotRequired();

        return AnsiConsole.Prompt(prompt);
    }

    internal static string DisplayQuestion(string question)
    {
        var response = AnsiConsole.Ask<string>($"[{White}]{question}[/]");
        return response;
    }

    internal static void DisplaySpinner(string waitMessage, int waitTimeInMs = 3000)
    {
        AnsiConsole.Status()
            .Spinner(Spinner.Known.Star)
            .Start($"[{White}]{waitMessage}[/]", ctx => { Thread.Sleep(waitTimeInMs); });
    }

    internal static async Task DisplayAsyncSpinner(string waitMessage, Task task)
    {
        await AnsiConsole.Status()
            .Spinner(Spinner.Known.Star)
            .StartAsync($"[{White}]{waitMessage}[/]", async ctx => { await task; });
    }
}