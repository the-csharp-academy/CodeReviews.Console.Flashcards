using Spectre.Console;
namespace CodeReviews.Console.Flashcards;

public sealed class AppController
{
    private readonly IAppView _appView;
    private readonly IStackController _stackController;
    public AppController(IAppView _view, IStackController _controller)
    {
        _appView = _view;
        _stackController = _controller;  
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
                    _stackController.Run();
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