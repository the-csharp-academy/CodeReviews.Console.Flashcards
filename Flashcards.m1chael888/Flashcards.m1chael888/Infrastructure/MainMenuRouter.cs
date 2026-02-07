using Flashcards.m1chael888.Controllers;
using Flashcards.m1chael888.Views;
using static Flashcards.m1chael888.Enums.MainMenuViewEnums;

namespace Flashcards.m1chael888.Infrastructure
{
    public interface IMainMenuRouter
    {
        void Route(StudyController studyController, ManageController manageController, IMainMenuView mainMenu);
    }
    public class MainMenuRouter : IMainMenuRouter
    {
        public void Route(StudyController studyController, ManageController manageController, IMainMenuView mainMenu)
        {
            while (true)
            {
                Console.Clear();
                var choice = mainMenu.ShowMenu();

                switch (choice)
                {
                    case MainMenuOption.Study:
                        studyController.HandleStudyMenu();
                        break;
                    case MainMenuOption.Manage:
                        manageController.HandleManageMenu();
                        break;
                    case MainMenuOption.Exit:
                        Environment.Exit(0);
                        break;
                }
            }
        }
    }
}
