using Flashcards._0lcm.DTOs;
using Flashcards._0lcm.Interfaces;
using Flashcards._0lcm.Models;
using Flashcards._0lcm.Services;

namespace Flashcards._0lcm.UserInterface;

internal class StudyUi(IStudyService studyService, IStackService stackService, IFlashcardService flashcardService)
{
    private const string ReturnOption = "* Back";
    private readonly IFlashcardService _flashcardService = flashcardService;
    private readonly IStackService _stackService = stackService;
    private readonly IStudyService _studyService = studyService;

    //------- Entry Point Menu -------
    internal void StudyArea()
    {
        while (true)
        {
            var choice = DisplayHelper.DisplayMenu<Enums.StudyAreaOption>();

            switch (choice)
            {
                case Enums.StudyAreaOption.Back:
                    return;
                case Enums.StudyAreaOption.Study:
                    Study();
                    break;
                case Enums.StudyAreaOption.ViewPastSessions:
                    ViewSessions();
                    break;
            }
        }
    }

    //------- Viewing Methods -------
    private StackDto? ChooseStack()
    {
        var stackMap = UtilityService.BuildStackMap(_stackService, ReturnOption);

        var stackChoice = DisplayHelper.DisplayPrompt(
            stackMap.Keys.ToList(),
            "Select a stack to study:");

        var stackDto = stackMap[stackChoice];

        return stackDto;
    }

    private void ViewSessions()
    {
        var hasLoaded = false;
        while (true)
        {
            Console.Clear();

            DisplayHelper.DisplayInfo("Scroll using <Up> and <Down>, press <Enter> to select.");
            DisplayHelper.DisplayInfo("Press 'Back' to return, or select a stack to continue.");
            if (!hasLoaded)
            {
                DisplayHelper.DisplaySpinner("Loading stacks..", 2000);
                hasLoaded = true;
            }

            var stackMap = UtilityService.BuildStackMap(_stackService, ReturnOption);

            var stackChoice = DisplayHelper.DisplayPrompt(
                stackMap.Keys.ToList(),
                "\nSelect a stack to view study sessions:");

            if (stackChoice == ReturnOption) return;

            var stackDto = stackMap[stackChoice];

            var sessions = UtilityService.BuildSessionRenderableRows(_studyService, stackDto!);
            DisplayHelper.DisplayRows(sessions);

            DisplayHelper.DisplayPrompt(new List<string> { "Back" }, "\nPress 'Back' to return.");
        }
    }

    //------- Study Methods -------
    private void Study()
    {
        Console.Clear();

        var stackDto = ChooseStack();
        if (stackDto == null) return;

        var flashcards = _studyService.RandomizeFlashcards(_flashcardService, stackDto);
        var studyCount = flashcards.Count;
        
        while (true)
        {
            StudyFlashcards(flashcards);
            
            if (flashcards.Count == 0) break;
        }

        Console.Clear();
        DisplayHelper.DisplaySpinner("Saving session..", 1500);
        _studyService.LogStudySession(studyCount, stackId: stackDto.StackId);
    }

    /// <summary>
    ///     Goes through each flashcard once from the provided list and shows them to the user.
    ///     fully studied flashcards are removed from the list, while flashcards that need review stay.
    /// </summary>
    private static void StudyFlashcards(List<FlashcardDto> flashcards)
    {
        for (var i = flashcards.Count - 1; i >= 0; i--)
        {
            Console.Clear();

            DisplayHelper.DisplayMessage($"\nName: {flashcards[i].Name}\n");
            DisplayHelper.DisplayMenu<Enums.StudyMenuOption>();
            DisplayHelper.DisplayMessage($"Value: {flashcards[i].Value}\n");

            var choice = DisplayHelper.DisplayMenu<Enums.StudyMenuOption2>();
            switch (choice)
            {
                case Enums.StudyMenuOption2.AssignFlashcardAsStudied:
                    flashcards.Remove(flashcards[i]);
                    break;
                case Enums.StudyMenuOption2.AssignFlashcardForReview:
                default:    
                    break;
            }
        }
    }
}