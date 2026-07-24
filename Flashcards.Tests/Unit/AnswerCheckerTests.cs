namespace CodeReviews.Console.Flashcards.Tests;

[TestFixture]
[Category("Unit")]
public sealed class AnswerCheckerTests
{
    private IAnswerChecker _answerChecker;
    private static readonly List<TestCaseData> testCases = [
        new TestCaseData("Warsaw", "Warsaw", true),
        new TestCaseData("warsaw", "Warsaw", true),
        new TestCaseData(" Warsaw ", "Warsaw", true),
        new TestCaseData("New   York", "New York", true),
        new TestCaseData("", "Warsaw", false),
        new TestCaseData("Cracow", "Warsaw", false)
    ];

    [SetUp]
    public void SetUp()
        => _answerChecker = new AnswerChecker();

    [TestCaseSource(nameof(testCases))]
    public void IsCorrect_ReturnsExpectedResult(string userAnswer, string correctAnswer, bool expected)
    {
        bool result = _answerChecker.IsCorrect(userAnswer, correctAnswer);
        Assert.That(result, Is.EqualTo(expected));
    }
}