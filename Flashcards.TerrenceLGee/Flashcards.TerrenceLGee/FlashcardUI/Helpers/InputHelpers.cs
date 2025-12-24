using System.ComponentModel.DataAnnotations;
using Spectre.Console;

namespace Flashcards.TerrenceLGee.FlashcardUI.Helpers;

public static class InputHelpers
{
    public static (bool, int?) ShowPaginatedItems<T>(
        List<T> items,
        string name,
        Action<List<T>> display,
        int pageSize = 10,
        bool returnId = false,
        string idFor = "")
    {
        if (items.Count == 0)
        {
            PressAnyKeyToContinueError($"You currently have no {name} available");
            return (false, null);
        }

        int? id = null;
        var pageIndex = 0;
        var pageCount = (int)Math.Ceiling(items.Count / (double)pageSize);

        while (true)
        {
            var pagedItems = items
                .Skip(pageIndex * pageSize)
                .Take(pageSize)
                .ToList();
            
            AnsiConsole.MarkupLine($"[skyblue3] " +
                                   $"Page {pageIndex + 1} of {pageCount} (showing {pagedItems.Count} of " +
                                   $"{items.Count})[/]");

            display(pagedItems);

            var prompt = new SelectionPrompt<Choices>()
                .Title("[darkslategray2]Navigate pages: [/]");

            if (pageIndex > 0)
            {
                prompt.AddChoice(Choices.Previous);
            }

            if (pageIndex < pageCount - 1)
            {
                prompt.AddChoice(Choices.Next);
            }
            
            prompt.AddChoice(Choices.Exit);

            var choice = AnsiConsole.Prompt(prompt);

            if (choice == Choices.Next && pageIndex < pageCount - 1)
            {
                pageIndex++;
                Console.Clear();
            }
            else if (choice == Choices.Previous && pageIndex > 0)
            {
                pageIndex--;
                Console.Clear();
            }
            else
            {
                break;
            }
        }
        if (returnId)
        {
            id = AnsiConsole.Ask<int>($"\n[darkseagreen3_1]Enter {idFor}: [/]");
        }
        PressAnyKeyToContinue();
        return (true, id);
    }

    public static void PressAnyKeyToContinue(string message = "")
    {
        AnsiConsole.MarkupLine($"[turquoise2]{message}[/]");
        AnsiConsole.MarkupLine("[purple4_1]Press any key to continue[/]");
        Console.ReadKey();
        AnsiConsole.Clear();
    }

    public static void PressAnyKeyToContinueError(string message = "")
    {
        AnsiConsole.MarkupLine($"[bold underline red]{message}[/]");
        AnsiConsole.MarkupLine($"[purple4_1]Press any key to continue[/]");
        Console.ReadKey();
        AnsiConsole.Clear();
    }

    public static void PressAnyKeyToContinueStudySession(string question)
    {
        AnsiConsole.MarkupLine($"[cyan2]{question}[/]");
        AnsiConsole.MarkupLine("[purple4_1]Press any key to answer[/]");
        Console.ReadKey();
        AnsiConsole.Clear();
    }
}

public enum Choices
{
    [Display(Name = "Previous Page")]
    Previous,
    [Display(Name = "Next Page")]
    Next,
    [Display(Name = "Exit")]
    Exit
}