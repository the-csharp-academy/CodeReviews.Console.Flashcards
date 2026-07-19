using Spectre.Console;
namespace CodeReviews.Console.Flashcards;

public sealed class AppController
{
    private readonly IAppView _appView;

    public AppController(IAppView _view)
    {
        _appView = _view;
    }

    public void Run()
    {
        bool isRunning = true;

        while (isRunning)
        {
            MainMenuOption selectedOption = _appView.DisplayMainMenu();

            switch (selectedOption)
            {
                case MainMenuOption.ManageStacks:
                    break;

                case MainMenuOption.ManageFlashcards:
                    break;

                case MainMenuOption.Exit:
                    isRunning = false;
                    break;

                default:
                    throw new ArgumentOutOfRangeException(
                        nameof(selectedOption),
                        selectedOption,
                        "Unknown menu option.");
            }
        }
        _appView.DisplayGoodbye();
    }
}