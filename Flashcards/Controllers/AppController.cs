using Spectre.Console;
namespace CodeReviews.Console.Flashcards;

public sealed class AppController
{
    private readonly IAppView _appView;
    private readonly IStackController _stackController;
    private readonly IFlashcardController _cardController;
    public AppController(IAppView view, IStackController stackController, IFlashcardController cardController)
    {
        _appView = view;
        _stackController = stackController;
        _cardController = cardController;
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
                    _cardController.Run();
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