using Flashcards.Controllers;
using Flashcards.Menus;

namespace Flashcards
{
    internal class Program
    {
        static void Main(string[] args)
        {
            var cardStackController = new CardStackController();
            var flashcardController = new FlashcardController();
            var studySessionController = new StudySessionController();

            var studyMenu = new StudyMenu(cardStackController, flashcardController, studySessionController);
            var flashcardMenu = new FlashcardMenu(flashcardController);
            var stackMenu = new CardStackMenu(cardStackController, flashcardMenu);
            var mainMenu = new MainMenu(stackMenu, studyMenu);

            mainMenu.Run();
        }
    }
}
