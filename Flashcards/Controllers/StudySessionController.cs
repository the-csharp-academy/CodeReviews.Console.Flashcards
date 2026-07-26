using Spectre.Console;

namespace CodeReviews.Console.Flashcards;

public sealed class StudySessionController : IStudySessionController
{
    private readonly IStudySessionsView _view;
    private readonly IStacksRepo _stacksRepo;
    private readonly IFlashcardsRepo _flashcardsRepo;
    private readonly IStudySessionsRepo _studySessionsRepo;
    private readonly IAnswerChecker _answerChecker;

    public StudySessionController(
        IStudySessionsView view,
        IStacksRepo stacksRepo,
        IFlashcardsRepo flashcardsRepo,
        IStudySessionsRepo studySessionsRepo,
        IAnswerChecker checker)
    {
        _view = view;
        _stacksRepo = stacksRepo;
        _flashcardsRepo = flashcardsRepo;
        _studySessionsRepo = studySessionsRepo;
        _answerChecker = checker;
    }
    public void Study()
    {
        string stackName = _view.AskForStackName();

        if (!TryGetCurrentStack(stackName, out CardStack currentStack))
            return;

        if (!TryGetCards(currentStack.StackId, out IReadOnlyList<Flashcard> cards))
            return;

        if (cards.Count == 0)
        {
            _view.DisplayMessage($"Stack '{currentStack.Name}' has no flashcards.");
            return;
        }

        int score = 0;

        for (int index = 0; index < cards.Count; index++)
        {
            Flashcard card = cards[index];

            _view.ShowQuestion(card.Question, index + 1, cards.Count);

            string userAnswer = _view.TakeAnswerFromUser();

            bool isCorrect = _answerChecker.IsCorrect(userAnswer, card.Answer);

            if (isCorrect) score++;

            _view.ShowAnswer(isCorrect, card.Answer);
        }

        if (!RepositoryHelpers.TryExecute(
            () => _studySessionsRepo.Add(new StudySession
            {
                StackId = currentStack.StackId,
                Score = score,
                TotalQuestions = cards.Count
            }),
            out Exception? exception))
        {
            _view.DisplayError($"The result could not be saved: {exception!.Message}");
            return;
        }

        _view.ShowFinalResult(score, cards.Count);
    }

    public void ViewHistory()
    {
        if (!TryGetStudySessions(out IReadOnlyList<StudySessionDTO> sessions))
            return;

        _view.DisplayAllStudySessions(sessions);
    }
    private bool TryGetCards(int stackId, out IReadOnlyList<Flashcard> cards)
    {
        cards = Array.Empty<Flashcard>();
        try
        {
            cards = _flashcardsRepo.GetAllByStackId(stackId);
            return true;
        }
        catch (Exception ex)
        {
            _view.DisplayError($"Could not retrieve flashcards: {ex.Message}");
            return false;
        }
    }

    private bool TryGetCurrentStack(string stackName, out CardStack currentStack)
    {
        currentStack = null!;
        try
        {
            CardStack? foundStack = _stacksRepo.GetStackByName(stackName);

            if (foundStack is null)
            {
                _view.DisplayError($"Stack '{stackName}' was not found.");
                return false;
            }

            currentStack = foundStack;
            return true;
        }
        catch (Exception ex)
        {
            _view.DisplayError($"Could not retrieve the stack: {ex.Message}");
            return false;
        }
    }

    private bool TryGetStudySessions(out IReadOnlyList<StudySessionDTO> sessions)
    {
        sessions = Array.Empty<StudySessionDTO>();
        try
        {
            sessions = _studySessionsRepo.GetAll();
            return true;
        }
        catch (Exception ex)
        {
            _view.DisplayError($"Could not retrieve sessions: {ex.Message}");
            return false;
        }
    }
}