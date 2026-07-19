namespace CodeReviews.Console.Flashcards;

public interface IAppView
{
    MainMenuOption DisplayMainMenu();
    void DisplayMessage(string message);
    void DisplayGoodbye();
}