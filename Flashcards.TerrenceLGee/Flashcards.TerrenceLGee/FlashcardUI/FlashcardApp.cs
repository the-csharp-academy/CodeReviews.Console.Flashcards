using Flashcards.TerrenceLGee.FlashcardUI.Helpers;
using Flashcards.TerrenceLGee.FlashcardUI.Interfaces;
using Flashcards.TerrenceLGee.FlashcardUI.Menus;
using Flashcards.TerrenceLGee.Services.Interfaces;
using Spectre.Console;

namespace Flashcards.TerrenceLGee.FlashcardUI;

public class FlashcardApp
{
    private readonly IStudyStackUi _stackUi;
    private readonly IFlashcardUi _flashcardUi;
    private readonly IStudySessionUi _sessionUi;
    private readonly IViewUi _viewUi;
    private readonly IStudyStackService _stackService;

    public FlashcardApp(
        IStudyStackUi stackUi,
        IFlashcardUi flashcardUi,
        IStudySessionUi sessionUi,
        IViewUi viewUi,
        IStudyStackService stackService)
    {
        _stackUi = stackUi;
        _flashcardUi = flashcardUi;
        _sessionUi = sessionUi;
        _viewUi = viewUi;
        _stackService = stackService;
    }

    public async Task RunAsync()
    {
        AnsiConsole.Write(
            new FigletText("Flashcards\n")
                .Centered()
                .Color(Color.Tan));
        
        InputHelpers.PressAnyKeyToContinue();
        
        var userFinished = false;
        var menuHelper = new MenuHelpers(_stackService);

        while (!userFinished)
        {
            var choice = await menuHelper.GetStackMenuChoice();
            switch (choice)
            {
                case StackMenu.AddStack:
                    await _stackUi.AddStudyStackAsync();
                    break;
                case StackMenu.UpdateStack:
                    await _stackUi.UpdateStudyStackAsync();
                    break;
                case StackMenu.DeleteStack:
                    await _stackUi.DeleteStudyStackAsync();
                    break;
                case StackMenu.ViewStacks:
                    await _viewUi.ViewStudyStacksAsync();
                    break;
                case StackMenu.ViewStack:
                    await _viewUi.ViewStudyStackAsync();
                    break;
                case StackMenu.GoToFlashcardMenu:
                    await HandleFlashcardMenu(menuHelper);
                    break;
                case StackMenu.GoToStudySessionMenu:
                    await HandleStudySessionMenu(menuHelper);
                    break;
                case StackMenu.Exit:
                    userFinished = true;
                    break;
            }
        }
        
        AnsiConsole.Write(
            new FigletText("Goodbye")
                .Centered()
                .Color(Color.Tan));
    }

    private async Task HandleFlashcardMenu(MenuHelpers menuHelper)
    {
        var userFinished = false;
        
        while (!userFinished)
        {
            var choice = await menuHelper.GetFlashcardMenuChoice();
            switch (choice)
            {
                case FlashcardMenu.AddFlashcard:
                    await _flashcardUi.AddFlashcardAsync();
                    break;
                case FlashcardMenu.UpdateFlashcard:
                    await _flashcardUi.UpdateFlashcardAsync();
                    break;
                case FlashcardMenu.DeleteFlashcard:
                    await _flashcardUi.DeleteFlashcardAsync();
                    break;
                case FlashcardMenu.ViewAllFlashcards:
                    await _viewUi.ViewFlashcardsAsync();
                    break;
                case FlashcardMenu.ViewFlashcard:
                    await _viewUi.ViewFlashcardAsync();
                    break;
                case FlashcardMenu.Exit:
                    userFinished = true;
                    break;
            }
        }
    }

    private async Task HandleStudySessionMenu(MenuHelpers menuHelper)
    {
        var userFinished = false;

        while (!userFinished)
        {
            var choice = await menuHelper.GetStudySessionMenu();
            switch (choice)
            {
                case StudySessionMenu.StartStudySession:
                    await _sessionUi.StartStudySessionAsync();
                    break;
                case StudySessionMenu.ViewPreviousStudySessions:
                    await _viewUi.ViewStudySessionsAsync();
                    break;
                case StudySessionMenu.ViewPreviousStudySession:
                    await _viewUi.ViewStudySessionAsync();
                    break;
                case StudySessionMenu.Exit:
                    userFinished = true;
                    break;
            }
        }
    }
}