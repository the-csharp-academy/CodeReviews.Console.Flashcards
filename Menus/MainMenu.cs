namespace Flashcards.Menus
{
    public class MainMenu
    {
        private readonly CardStackMenu stackMenu;
        private readonly StudyMenu studyMenu;

        public MainMenu(CardStackMenu stackMenu, StudyMenu studyMenu)
        {
            this.stackMenu = stackMenu;
            this.studyMenu = studyMenu;
        }

        public void Run()
        {
            while (true)
            {
                var result = UI.GetActions(new List<string>
                {
                    "Study",
                    "Manage stacks",
                    "Exit"
                });

                if (result == "Study")
                    studyMenu.Run();

                if (result == "Manage stacks")
                    stackMenu.Run();

                if (result == "Exit")
                    return;
            }
        }
    }
}
