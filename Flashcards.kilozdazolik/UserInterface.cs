using Spectre.Console;
using Flashcards.kilozdazolik.Enums;
using Flashcards.kilozdazolik.Controller;

namespace Flashcards.kilozdazolik;

public class UserInterface
{
    private static StackController _stackController = new();
    private static FlashcardController _flashcardController = new();
    private static SessionController _sessionController = new();
    internal static void MainMenu()
    {
        while (true)
        {
            var choice = AnsiConsole.Prompt(new SelectionPrompt<MenuAction>().Title("What do you want to do [green]next[/]?")
                .PageSize(10)
                .MoreChoicesText("[grey](Move up and down to choose an option)[/]")
                .AddChoices(Enum.GetValues<MenuAction>()));

            switch (choice)
            {
                case MenuAction.Exit:
                    Environment.Exit(0);
                    break;
                case MenuAction.ViewAllStacks:
                    _stackController.ViewAllStacks();
                    break;
                case MenuAction.ViewFlashcards:
                    _flashcardController.ViewFlashcards();
                    break;
                case MenuAction.ManageStacks:
                    ManageStacks();
                    break;
                case MenuAction.ManageFlashcards:
                    ManageFlashcards();
                    break;
                case MenuAction.Study:
                    _sessionController.StartSession();
                    break;
                case MenuAction.ViewStudySessions:
                    ManageSessions();
                    break;
            }
        }
    }

    private static void ManageFlashcards()
    {
        var choice = AnsiConsole.Prompt(
            new SelectionPrompt<ActionType>()
                .Title("What do you want to do [green]next[/]?")
                .PageSize(10)
                .MoreChoicesText("[grey](Move up and down to choose an option)[/]")
                .AddChoices(Enum.GetValues<ActionType>()));

        switch (choice)
        {
            case ActionType.Insert:
                _flashcardController.CreateFlashcard();
                break;
            case ActionType.Update:
                _flashcardController.EditFlashcard();
                break;
            case ActionType.Delete:
                _flashcardController.DeleteFlashcard();
                break;
        }
    }

    private static void ManageStacks()
    {
        var choice = AnsiConsole.Prompt(
            new SelectionPrompt<ActionType>()
                .Title("What do you want to do [green]next[/]?")
                .PageSize(10)
                .MoreChoicesText("[grey](Move up and down to choose an option)[/]")
                .AddChoices(Enum.GetValues<ActionType>()));

        switch (choice)
        {
            case ActionType.Insert:
                _stackController.CreateStack();
                break;
            case ActionType.Update:
                _stackController.EditStack();
                break;
            case ActionType.Delete:
                _stackController.DeleteStack();
                break;
        }
    }

    private static void ManageSessions()
    {
        var choice = AnsiConsole.Prompt(
            new SelectionPrompt<ViewType>()
                .Title("What do you want to do [green]next[/]?")
                .PageSize(10)
                .MoreChoicesText("[grey](Move up and down to choose an option)[/]")
                .AddChoices(Enum.GetValues<ViewType>()));

        switch (choice)
        {
            case ViewType.All:
                _sessionController.ViewAllSession();
                break;
            case ViewType.ByYear:
                break;
        }
    }
    
}