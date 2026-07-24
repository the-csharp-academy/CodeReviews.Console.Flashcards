namespace CodeReviews.Console.Flashcards;

public interface IStudySessionsView
{
    string AskForStackName();
    string TakeAnswerFromUser();
    void ShowQuestion(string question, int currentNumber, int totalQuestions);
    void ShowAnswer(bool isCorrect, string answer);
    void ShowFinalResult(int score, int totalQuestions);
    void DisplayMessage(string message);
    void DisplayError(string message);
    void DisplayAllStudySessions(IReadOnlyList<StudySessionDTO> studies);
}