using Flashcards.TerrenceLGee.FlashcardUI.Helpers;
using Flashcards.TerrenceLGee.FlashcardUI.Interfaces;
using Flashcards.TerrenceLGee.Services.Interfaces;

namespace Flashcards.TerrenceLGee.FlashcardUI;

public class ViewUi : IViewUi
{
    private readonly IStudyStackService _stackService;
    private readonly IFlashcardService _flashcardService;
    private readonly IStudySessionService _studySessionService;

    public ViewUi(
        IStudyStackService stackService,
        IFlashcardService flashcardService,
        IStudySessionService studySessionService)
    {
        _stackService = stackService;
        _flashcardService = flashcardService;
        _studySessionService = studySessionService;
    }


    public async Task ViewStudyStacksAsync()
    {
        var stacks = await _stackService.GetStacksAsync();

        if (stacks.Count == 0)
        {
            InputHelpers
                .PressAnyKeyToContinueError("There are no stacks currently available for display");
            return;
        }

        InputHelpers.ShowPaginatedItems(stacks, "study stacks", ViewHelpers.DisplayStudyStacks);
    }

    public async Task ViewStudyStackAsync()
    {
        var stacks = await _stackService.GetStacksAsync();

        if (stacks.Count == 0)
        {
            InputHelpers
                .PressAnyKeyToContinueError("There are no stacks currently available for display");
            return;
        }

        var (isDisplayed, id) =
            InputHelpers
                .ShowPaginatedItems(stacks, "study stacks", ViewHelpers.DisplayStudyStacks, 10, true, "the id of the stack you wish to view");

        if (!isDisplayed)
        {
            return;
        }

        if (!id.HasValue)
        {
            InputHelpers
                .PressAnyKeyToContinueError("There was an error retrieving the stack for viewing");
            return;
        }

        var stack = await _stackService.GetStackAsync(id.Value);
        if (stack is null)
        {
            InputHelpers
                .PressAnyKeyToContinueError($"There was an error retrieving stack {id.Value} for viewing");
            return;
        }
        
        ViewHelpers.DisplayStudyStack(stack);
    }

    public async Task ViewFlashcardsAsync()
    {
        var stackId =
            await new MenuHelpers(_stackService).GetStackIdAsync(
                "Please choose the name of the stack these flashcards belong to");
        var flashcards = await _flashcardService.GetFlashcardsAsync(stackId);

        if (flashcards.Count == 0)
        {
            InputHelpers
                .PressAnyKeyToContinueError($"There are no flashcards associated with stack {stackId}");
            return;
        }

        InputHelpers.ShowPaginatedItems(flashcards, $"flashcards associated with stack {stackId}", ViewHelpers.DisplayFlashcards);
    }

    public async Task ViewFlashcardAsync()
    {
        var stackId =
            await new MenuHelpers(_stackService).GetStackIdAsync(
                "Please choose the name of the stack this flashcard belongs to");
        var flashcards = await _flashcardService.GetFlashcardsAsync(stackId);

        if (flashcards.Count == 0)
        {
            if (flashcards.Count == 0)
            {
                InputHelpers
                    .PressAnyKeyToContinueError($"There are no flashcards associated with stack {stackId}");
                return;
            }
        }

        var (isDisplayed, id) = InputHelpers
            .ShowPaginatedItems(flashcards, $"flashcards associated with stack {stackId}",
                ViewHelpers.DisplayFlashcards, 10, true, "the position of the flashcard you wish to view");

        if (!isDisplayed)
        {
            return;
        }

        if (!id.HasValue)
        {
            InputHelpers
                .PressAnyKeyToContinueError($"Unable to retrieve the flashcard for stack {stackId}");
            return;
        }

        var flashcard = await _flashcardService.GetFlashcardAsync(stackId, id.Value);

        if (flashcard is null)
        {
            InputHelpers
                .PressAnyKeyToContinueError($"Error retrieving the flashcard for stack {stackId}");
            return;
        }
        
        ViewHelpers.DisplayFlashCard(flashcard);
        InputHelpers
            .PressAnyKeyToContinue();
    }

    public async Task ViewStudySessionsAsync()
    {
        var stackId =
            await new MenuHelpers(_stackService).GetStackIdAsync(
                "Please choose the name of the stack these study sessions are associated with");
        var sessions = await _studySessionService.GetStudySessionsAsync(stackId);

        if (sessions.Count == 0)
        {
            InputHelpers
                .PressAnyKeyToContinueError("There are currently no study sessions available for display");
            return;
        }

        InputHelpers.ShowPaginatedItems(sessions, $"study sessions for stack {stackId}",
            ViewHelpers.DisplayStudySessions);
    }

    public async Task ViewStudySessionAsync()
    {
        var stackId =
            await new MenuHelpers(_stackService).GetStackIdAsync(
                "Please choose the name of the stack this study session is associated with");
        var stackName = await _stackService.GetStackNameAsync(stackId);
        
        var sessions = await _studySessionService.GetStudySessionsAsync(stackId);

        if (sessions.Count == 0)
        {
            InputHelpers
                .PressAnyKeyToContinueError("There are currently no study sessions available");
            return;
        }

        var (isDisplay, id) = InputHelpers
            .ShowPaginatedItems(sessions, $"study sessions for stack {stackId}", ViewHelpers.DisplayStudySessions, 10,
                true, $"the id of the study session for stack {stackId}");

        if (!isDisplay)
        {
            return;
        }

        if (!id.HasValue)
        {
            InputHelpers
                .PressAnyKeyToContinueError($"Study session not found");
            return;
        }

        var session = await _studySessionService.GetStudySessionAsync(stackId, id.Value);
        if (session is null)
        {
            InputHelpers
                .PressAnyKeyToContinueError($"There was an error retrieving the study session {id.Value} for stack: {stackId}");
            return;
        }
        
        ViewHelpers.DisplayStudySession(session, stackName);
        InputHelpers.PressAnyKeyToContinue();
    }
}