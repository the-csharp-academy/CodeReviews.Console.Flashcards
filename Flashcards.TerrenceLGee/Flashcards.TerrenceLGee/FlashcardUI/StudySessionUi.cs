using System.Diagnostics;
using Flashcards.TerrenceLGee.DTOs.FlashcardDTOs;
using Flashcards.TerrenceLGee.DTOs.SessionFlashcardDTOs;
using Flashcards.TerrenceLGee.DTOs.StudySessionDTOs;
using Flashcards.TerrenceLGee.FlashcardUI.Helpers;
using Flashcards.TerrenceLGee.FlashcardUI.Interfaces;
using Flashcards.TerrenceLGee.Services.Interfaces;
using Spectre.Console;

namespace Flashcards.TerrenceLGee.FlashcardUI;

public class StudySessionUi : IStudySessionUi
{
    private readonly IStudySessionService _sessionService;
    private readonly IStudyStackService _stackService;
    private readonly IFlashcardService _flashcardService;
    private readonly ISessionFlashcardService _sessionFlashcardService;

    public StudySessionUi(
        IStudySessionService sessionService, 
        IStudyStackService stackService,
        IFlashcardService flashcardService,
        ISessionFlashcardService sessionFlashcardService)
    {
        _sessionService = sessionService;
        _stackService = stackService;
        _flashcardService = flashcardService;
        _sessionFlashcardService = sessionFlashcardService;
    }


    public async Task StartStudySessionAsync()
    {
        var stackId =
            await new MenuHelpers(_stackService).GetStackIdAsync(
                "Please choose the name of the stack that you wish to study");

        if (stackId == -1)
        {
            InputHelpers
                .PressAnyKeyToContinueError("Stack not found");
            return;
        }

        var stackName = await _stackService.GetStackNameAsync(stackId);

        var flashcards = await _flashcardService.GetFlashcardsAsync(stackId);
        
        if (flashcards.Count == 0)
        {
            InputHelpers
                .PressAnyKeyToContinueError($"There are no flashcards associated with stack {stackId}\nUnable to begin study session");
            return;
        }

        var wantsACertainNumber = await AnsiConsole
            .ConfirmAsync($"[teal]There are {flashcards.Count} flashcards in this stack, " +
                          $"would you like to specify the number of flashcards to be quizzed on?[/]\n");

        var questionsToAsk = int.MaxValue;
        
        if (wantsACertainNumber)
        {
            while (questionsToAsk > flashcards.Count)
            {
                questionsToAsk = AnsiConsole.Ask<int>($"[teal]Enter the total number of flashcards to be quizzed" +
                                                      $" on (less than or equal to {flashcards.Count}, and greater than 0): [/]");
            }
        }

        var wantsRandomQuestions = await AnsiConsole.ConfirmAsync("[teal]Would you like for your flashcards to be randomized? [/]");

        var startStudySession = await AnsiConsole.ConfirmAsync("[teal]\nAre you sure you want to begin this study session? [/]");

        if (!startStudySession)
        {
            InputHelpers
                .PressAnyKeyToContinue("Study session aborted\nReturning to the previous menu");
            return;
        }
        
        InputHelpers
            .PressAnyKeyToContinue("Study session will begin when you are ready");

        var stopwatch = new Stopwatch();
        int[] results;
        List<SessionFlashcardDto> sessionFlashcards;
        
        stopwatch.Start();
        if (wantsACertainNumber)
        {
            var limitedFlashcards = flashcards.GetRange(0, questionsToAsk);
            (results, sessionFlashcards) = Study(limitedFlashcards, wantsRandomQuestions);
        }
        else
        {
            (results, sessionFlashcards) = Study(flashcards, wantsRandomQuestions);
        }
        
        stopwatch.Stop();

        var duration = new TimeSpan(stopwatch.Elapsed.Hours, stopwatch.Elapsed.Minutes, stopwatch.Elapsed.Seconds);
        var totalQuestions = results[0];
        var correct = results[1];
        var incorrect = results[2];
        var score = ((double)correct / totalQuestions) * 100.0;

        var studySession = new CreateStudySessionDto
        {
            StackId = stackId,
            TotalQuestions = totalQuestions,
            Correct = correct,
            Incorrect = incorrect,
            Score = score,
            SessionDuration = duration
        };

        var sessionId = await _sessionService.AddStudySessionAsync(studySession);

        if (sessionId <= 0)
        {
            InputHelpers
                .PressAnyKeyToContinueError($"There was an error associated the study session with stack {stackId}");
            return;
        }
        
        SetStudySessionIdForSessionFlashcards(sessionFlashcards, sessionId);

        if (!await SaveStudySessionFlashcards(sessionFlashcards))
        {
            InputHelpers
                .PressAnyKeyToContinueError("Study session saved but unable to save the associated flashcards");
        }
        
        InputHelpers
            .PressAnyKeyToContinue("[teal]The results of your study session[/]");
        
        ViewHelpers.DisplayStudySession(studySession, sessionFlashcards, stackName);
        InputHelpers.PressAnyKeyToContinue();
    }

    private static (int[], List<SessionFlashcardDto>) Study(List<RetrievedFlashcardDto> flashcards, bool wantsRandom)
    {
        var data = new int[3];
        var sessionFlashcards = new List<SessionFlashcardDto>();
        
        data[0] = flashcards.Count;

        var index = 0;
        var random = new Random();
        var limit = flashcards.Count;
        var previouslySeen = new List<int>();

        if (wantsRandom)
        {
            index = random.Next(limit);
        }

        for (var i = 0; i < limit; i++)
        {
            var question = flashcards[index].Question;
            InputHelpers
                .PressAnyKeyToContinueStudySession(question);
            var answer = AnsiConsole.Ask<string>("[fuchsia]Answer: [/]").Trim();
            var actualAnswer = flashcards[index].Answer;

            var displayPosition = i + 1;
            var sessionFlashcard = new SessionFlashcardDto
            {
                FlashcardId = flashcards[index].Id,
                Question = question,
                Answer = actualAnswer,
                UserAnswer = answer,
                DisplayPosition = displayPosition,
            };
            
            if (IsCorrectAnswer(actualAnswer, answer))
            {
                data[1]++;
                InputHelpers.PressAnyKeyToContinue("CORRECT!");
                sessionFlashcard.IsCorrect = true;
            }
            else
            {
                data[2]++;
                InputHelpers.PressAnyKeyToContinue("INCORRECT");
                sessionFlashcard.IsCorrect = false;
            }

            sessionFlashcards.Add(sessionFlashcard);
            
            if (wantsRandom)
            {
                var oldIndex = index;
                previouslySeen.Add(oldIndex);
                while (true)
                {
                    if (i == limit - 1) break;
                    index = random.Next(limit);
                    if (!previouslySeen.Contains(index)) break;
                }
            }
            else
            {
                index++;
            }
        }

        return (data, sessionFlashcards);
    }

    private static bool IsCorrectAnswer(string actualAnswer, string userAnswer)
    {
        var preparedActualAnswer = actualAnswer.Replace(" ", "");
        var preparedUserAnswer = userAnswer.Replace(" ", "");
        return preparedActualAnswer.Equals(preparedUserAnswer, StringComparison.OrdinalIgnoreCase);
    }

    private async Task<bool> SaveStudySessionFlashcards(List<SessionFlashcardDto> sessionFlashcards)
    {
        foreach (var sessionFlashcard in sessionFlashcards)
        {
            var result = await _sessionFlashcardService.AddSessionFlashcardAsync(sessionFlashcard);
            if (result != 1) return false;
        }

        return true;
    }

    private static void SetStudySessionIdForSessionFlashcards(List<SessionFlashcardDto> sessionFlashcards, int sessionId)
    {
        foreach (var sessionFlashcard in sessionFlashcards)
        {
            sessionFlashcard.SessionId = sessionId;
        }
    }
}