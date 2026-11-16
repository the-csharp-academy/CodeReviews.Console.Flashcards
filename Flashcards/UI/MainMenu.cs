using Spectre.Console;

namespace Flashcards.UI
{
    internal class MainMenu
    {
        private readonly StackMenu _stackMenu;
        private readonly FlashcardMenu _flashcardMenu;
        private readonly StudySessionMenu _studySessionMenu;
        private readonly ReportsMenu _reportsMenu;
        public MainMenu(StackMenu stackMenu, FlashcardMenu flashcardMenu, StudySessionMenu studySessionMenu, ReportsMenu reportsMenu)
        {
            _stackMenu = stackMenu;
            _flashcardMenu = flashcardMenu;
            _studySessionMenu = studySessionMenu;
            _reportsMenu = reportsMenu;
        }

        internal async Task Show()
        {
            bool isRunning = true;

            while (isRunning)
            {
                AnsiConsole.Clear();

                var choice = AnsiConsole.Prompt(new SelectionPrompt<MainMenuChoices>()
                    .Title("Main Menu")
                    .UseConverter(c => c switch
                    {
                        MainMenuChoices.StartStudySession => "Study session",
                        MainMenuChoices.ManageStacks => "Manage Stacks",
                        MainMenuChoices.ManageFlashcards => "Manage Flashcards",
                        MainMenuChoices.Reports => "Reports",
                        MainMenuChoices.Exit => "Exit"
                    })
                    .AddChoices(Enum.GetValues<MainMenuChoices>()));

                switch (choice)
                {
                    case MainMenuChoices.StartStudySession:
                        await _studySessionMenu.Show();
                        break;
                    case MainMenuChoices.ManageStacks:
                        await _stackMenu.Show();
                        break;
                    case MainMenuChoices.ManageFlashcards:
                        await _flashcardMenu.Show();
                        break;
                    case MainMenuChoices.Reports:
                        await _reportsMenu.Show();
                        break;
                    case MainMenuChoices.Exit:
                        isRunning = false;
                        break;

                }
            }
        }
    }
}

enum MainMenuChoices
{
    StartStudySession,
    ManageStacks,
    ManageFlashcards,
    Reports,
    Exit
}
