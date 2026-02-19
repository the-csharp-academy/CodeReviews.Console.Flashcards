using Flashcards._0lcm.Logging;
using Flashcards._0lcm.Models;
using Microsoft.Extensions.Logging;

namespace Flashcards._0lcm.UserInterface;

internal class ConsoleUi(StackUi stackUi, FlashcardUi flashcardUi, StudyUi studyUi)
{
    private static readonly ILogger Logger = AppLogger.CreateLogger<ConsoleUi>();
    private readonly FlashcardUi _flashcardUi = flashcardUi;
    private readonly StackUi _stackUi = stackUi;
    private readonly StudyUi _studyUi = studyUi;

    internal void MainMenu()
    {
        while (true)
            try
            {
                Console.Clear();
                HandleMainMenuChoice();
            }
            catch (Exception ex)
            {
                Logger.LogWarning(ex, "Exception caught in the main menu loop.");
            }
    }

    private void HandleMainMenuChoice()
    {
        var choice = DisplayHelper.DisplayMenu<Enums.MainMenuOption>();

        switch (choice)
        {
            case Enums.MainMenuOption.StackArea:
                _stackUi.StackArea();
                break;
            case Enums.MainMenuOption.FlashcardArea:
                _flashcardUi.FlashcardArea();
                break;
            case Enums.MainMenuOption.StudyArea:
                _studyUi.StudyArea();
                break;
            case Enums.MainMenuOption.Exit:
                ExitApplication();
                break;
        }
    }

    private static void ExitApplication()
    {
        DisplayHelper.DisplaySpinner("Exiting application..", 2000);

        Environment.Exit(0);
    }
}