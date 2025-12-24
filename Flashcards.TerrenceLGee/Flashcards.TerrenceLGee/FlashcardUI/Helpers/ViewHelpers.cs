using Flashcards.TerrenceLGee.DTOs.FlashcardDTOs;
using Flashcards.TerrenceLGee.DTOs.SessionFlashcardDTOs;
using Flashcards.TerrenceLGee.DTOs.StudySessionDTOs;
using Flashcards.TerrenceLGee.DTOs.StudyStackDTOs;
using Flashcards.TerrenceLGee.Extensions;
using Spectre.Console;

namespace Flashcards.TerrenceLGee.FlashcardUI.Helpers;

public static class ViewHelpers
{
    public static void DisplayStudyStacks(List<RetrievedStudyStackDto> stacks)
    {
        AnsiConsole.WriteLine();

        var table = new Table();
        table.AddColumn("Id");
        table.AddColumn("Subject");
        table.AddColumn("Stack name");

        foreach (var stack in stacks)
        {
            table.AddRow(
                stack.Id.ToString(),
                stack.Subject.GetDisplayName(),
                stack.Name);
        }
        
        AnsiConsole.Write(table);
    }

    public static void DisplayStudyStack(RetrievedStudyStackDto stack)
    {
        AnsiConsole.WriteLine();
        AnsiConsole.MarkupLine("[bold underline darkkhaki]Stack Information\n[/]");
        AnsiConsole.MarkupLine($"[darkcyan]Subject:[/] [darkgoldenrod]{stack.Subject.GetDisplayName()}[/]");
        AnsiConsole.MarkupLine($"[darkcyan]Name:[/] [darkgoldenrod]{stack.Name}[/]");
        AnsiConsole.MarkupLine($"[darkcyan]Number of flashcards associated with this stack:[/] [darkgoldenrod]{stack.Flashcards.Count}[/]");
        AnsiConsole.MarkupLine($"[darkcyan]Number of times this stack has been studied:[/] [darkgoldenrod]{stack.StudySessions.Count}[/]");
        

        var flashcards = stack.Flashcards;
        if (flashcards.Count == 0)
        {
            InputHelpers.PressAnyKeyToContinue();
            return;
        }
        
        InputHelpers.PressAnyKeyToContinue();

        AnsiConsole.MarkupLine($"[rosybrown]Flashcards associated with the {stack.Name} stack[/]");
        InputHelpers.ShowPaginatedItems(flashcards, $"flashcards associated with the[/] [darkgoldenrod]{stack.Name} stack",
            DisplayFlashcards);

        var studySessions = stack.StudySessions;
        if (studySessions.Count == 0)
        {
            return;
        }
        
        AnsiConsole.MarkupLine($"[mediumpurple2_1]Study sessions associated with the {stack.Name} stack[/]");
        InputHelpers.ShowPaginatedItems(studySessions, $"study sessions associated with the {stack.Name} stack",
            DisplayStudySessions);
    }

    public static void DisplayFlashcards(List<RetrievedFlashcardDto> flashcards)
    {
        AnsiConsole.WriteLine();
        var table = new Table();
        table.AddColumn("Card position");
        table.AddColumn("Question");
        table.AddColumn("Answer");

        foreach (var flashcard in flashcards)
        {
            table.AddRow(
                flashcard.Position.ToString(),
                flashcard.Question,
                flashcard.Answer);
        }
        
        AnsiConsole.Write(table);
    }

    public static void DisplayFlashCard(RetrievedFlashcardDto flashcard)
    {
        AnsiConsole.WriteLine();
        AnsiConsole.MarkupLine("[bold underline darkkhaki]Flashcard Information\n[/]");
        AnsiConsole.MarkupLine($"[darkcyan]Card number:[/] [darkgoldenrod]{flashcard.Position}[/]");
        AnsiConsole.MarkupLine($"[darkcyan]Question:[/] [darkgoldenrod]{flashcard.Question}[/]");
        AnsiConsole.MarkupLine($"[darkcyan]Answer:[/] [darkgoldenrod]{flashcard.Answer}[/]");
    }
    
    public static void DisplayStudySessions(List<RetrievedStudySessionDto> sessions)
    {
        AnsiConsole.WriteLine();
        
        var table = new Table();
        table.AddColumn("Study Session #");
        table.AddColumn("Total Questions");
        table.AddColumn("Total Correct");
        table.AddColumn("Total Incorrect");
        table.AddColumn("Score");
        table.AddColumn("Study Session Duration");

        foreach (var session in sessions)
        {
            table.AddRow(
                session.Id.ToString(),
                session.TotalQuestions.ToString(),
                session.Correct.ToString(),
                session.Incorrect.ToString(),
                session.Score.ToString("F1"),
                session.SessionDuration.ToString());
        }
        
        AnsiConsole.Write(table);
    }


    public static void DisplayStudySession(RetrievedStudySessionDto session, string stackName)
    {
        AnsiConsole.WriteLine();
        
        AnsiConsole.MarkupLine("[bold underline darkkhaki]Study Session Information\n[/]");
        AnsiConsole.MarkupLine($"[darkcyan]Stack:[/] [darkgoldenrod]{stackName}[/]");
        AnsiConsole.MarkupLine($"[darkcyan]Total Questions:[/] [darkgoldenrod]{session.TotalQuestions}[/]");
        AnsiConsole.MarkupLine($"[darkcyan]Total Correct:[/] [darkgoldenrod]{session.Correct}[/]");
        AnsiConsole.MarkupLine($"[darkcyan]Total Incorrect:[/] [darkgoldenrod]{session.Incorrect}[/]");
        AnsiConsole.MarkupLine($"[darkcyan]Score:[/] [darkgoldenrod]{session.Score:F1}[/]");
        AnsiConsole.MarkupLine($"[darkcyan]Study Session Duration:[/] [darkgoldenrod]{session.SessionDuration:c}[/]");

        var sessionFlashcards = session.SessionFlashcards;
        
        if (sessionFlashcards.Count == 0) return;

        AnsiConsole.MarkupLine("\n[bold underline darkkhaki]Flashcards studied this session (Incorrectly answered in red)[/]");

        InputHelpers.ShowPaginatedItems(sessionFlashcards, "flashcards studied during this study session", DisplaySessionFlashcards);
    }

    public static void DisplayStudySession(CreateStudySessionDto session, List<SessionFlashcardDto> sessionFlashcards, string stackName)
    {
        AnsiConsole.WriteLine();
        
        AnsiConsole.MarkupLine("[bold underline darkkhaki]Study Session Information\n[/]");
        AnsiConsole.MarkupLine($"[darkcyan]Stack:[/] [darkgoldenrod]{stackName}[/]");
        AnsiConsole.MarkupLine($"[darkcyan]Total Questions:[/] [darkgoldenrod]{session.TotalQuestions}[/]");
        AnsiConsole.MarkupLine($"[darkcyan]Total Correct:[/] [darkgoldenrod]{session.Correct}[/]");
        AnsiConsole.MarkupLine($"[darkcyan]Total Incorrect:[/] [darkgoldenrod]{session.Incorrect}[/]");
        AnsiConsole.MarkupLine($"[darkcyan]Score:[/] [darkgoldenrod]{session.Score:F1}[/]");
        AnsiConsole.MarkupLine($"[darkcyan]Study Session Duration:[/] [darkgoldenrod]{session.SessionDuration:c}[/]");

        if (sessionFlashcards.Count == 0) return;

        AnsiConsole.MarkupLine("\n[bold underline darkkhaki]Flashcards studied this session (Incorrectly answered in red)[/]");

        InputHelpers.ShowPaginatedItems(sessionFlashcards, "flashcards studied during this study session", DisplaySessionFlashcards);
    }

    private static void DisplaySessionFlashcards(List<SessionFlashcardDto> sessionFlashcards)
    {
        AnsiConsole.WriteLine();
        
        var table = new Table();
        table.AddColumn("Flashcard #");
        table.AddColumn("Question");
        table.AddColumn("Answer");
        table.AddColumn("Your answered");
        table.AddColumn("Answered correctly?");

        foreach (var flashcard in sessionFlashcards)
        {
            if (flashcard.IsCorrect)
            {
                table.AddRow(
                    $"{flashcard.DisplayPosition}",
                    $"{flashcard.Question}",
                    $"{flashcard.Answer}",
                    $"{flashcard.UserAnswer}",
                    $"{flashcard.IsCorrect}");
            }
            else
            {
                table.AddRow(
                    $"[bold red]{flashcard.DisplayPosition}[/]",
                    $"[bold red]{flashcard.Question}[/]",
                    $"[bold red]{flashcard.Answer}[/]",
                    $"[bold red]{flashcard.UserAnswer}[/]",
                    $"[bold red]{flashcard.IsCorrect}[/]");
            }
        }
        
        AnsiConsole.Write(table);
    }
}