using System.Text;
using System.Text.RegularExpressions;

namespace CodeReviews.Console.Flashcards;

public interface IAnswerChecker
{
    bool IsCorrect(string userAnswer, string correctAnswer);
}

public sealed class AnswerChecker : IAnswerChecker
{
    public bool IsCorrect(string userAnswer, string correctAnswer)
     => string.Equals(
            Normalize(userAnswer),
            Normalize(correctAnswer),
            StringComparison.OrdinalIgnoreCase);


    private static string Normalize(string answer)
    {
        if (string.IsNullOrWhiteSpace(answer))
            return string.Empty;

        string normalized = answer.Normalize(NormalizationForm.FormC);
        normalized = normalized.Trim();

        return Regex.Replace(normalized, @"\s+", " ");
    }
}