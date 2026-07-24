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

        CardStack? currentStack;

        try
        {
            currentStack = _stacksRepo.GetStackByName(stackName);
        }
        catch (Exception ex)
        {
            _view.DisplayError($"Could not retrieve the stack: {ex.Message}");
            return;
        }

        if (currentStack == null)
        {
            _view.DisplayError($"Stack '{stackName}' was not found.");
            return;
        }

        IReadOnlyList<Flashcard> cards;

        try
        {
            cards = _flashcardsRepo.GetAllByStackId(currentStack.StackId);
        }
        catch (Exception ex)
        {
            _view.DisplayError($"Could not retrieve flashcards: {ex.Message}");
            return;
        }

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

        var session = new StudySession
        {
            StackId = currentStack.StackId,
            Score = score,
            TotalQuestions = cards.Count
        };

        try
        {
            _studySessionsRepo.Add(session);
        }
        catch (Exception ex)
        {
            _view.DisplayError($"The result could not be saved: {ex.Message}");
            return;
        }

        _view.ShowFinalResult(score, cards.Count);
    }

    public void ViewHistory()
    {
        IReadOnlyList<StudySessionDTO> sessions;

        try
        {
            sessions = _studySessionsRepo.GetAll();
        }
        catch (Exception e)
        {
            _view.DisplayError($"Could not retrieve sessions: {e.Message}");
            return;
        }

        _view.DisplayAllStudySessions(sessions);
    }
}