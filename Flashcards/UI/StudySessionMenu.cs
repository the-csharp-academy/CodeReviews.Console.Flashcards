using Flashcards.DTOs;
using Flashcards.Services;
using Spectre.Console;

namespace Flashcards.UI
{
    internal class StudySessionMenu
    {
        private readonly StackService _stackService;
        private readonly StudySessionUI _studySessionUI;
        private readonly StudySessionService _studySessionService;
        private readonly ReportingService _reportingService;

        public StudySessionMenu(StackService stackService, StudySessionUI studySessionUI, StudySessionService studySessionService, ReportingService reportingService)
        {
            _stackService = stackService;
            _studySessionUI = studySessionUI;
            _studySessionService = studySessionService;
            _reportingService = reportingService;
        }
        internal async Task Show()
        {
            bool show = true;

            while (show)
            {
                AnsiConsole.Clear();

                var choice = AnsiConsole.Prompt(new SelectionPrompt<StudySessionMenuOptions>()
                    .Title("What do you want to do?")
                    .UseConverter(x => x switch
                    {
                        StudySessionMenuOptions.Start => "Start a new study session",
                        StudySessionMenuOptions.Back => "Back"
                    })
                    .AddChoices(Enum.GetValues<StudySessionMenuOptions>()));

                switch (choice)
                {
                    case StudySessionMenuOptions.Start:
                        await StudySessionStackMenu();
                        break;
                    case StudySessionMenuOptions.Back:
                        show = false;
                        break;
                }
            }
        }

        internal async Task StudySessionStackMenu()
        {
            AnsiConsole.Clear();
            var stacks = await _stackService.GetAllStacksWithFlashcardsAsync();

            var stackChoice = AnsiConsole.Prompt(new SelectionPrompt<StackDTO>()
                .Title("Please select a stack you want to study.")
                .UseConverter(x =>
                {
                    return $"{x.Name}";
                })
                .AddChoices(stacks));

            await _studySessionUI.Show(stackChoice);
        }


    }
}

enum StudySessionMenuOptions
{
    Start,
    Back
}


