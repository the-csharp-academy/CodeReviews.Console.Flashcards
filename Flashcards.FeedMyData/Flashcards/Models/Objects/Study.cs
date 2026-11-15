namespace Flashcards;

internal class Study
{
  internal int StudyId { get; private set; }
  internal int StackId { get; private set; }
  internal DateTime Date { get; private set; }
  internal int Score { get; private set; }

  internal Study(int sutdyId, int stackId, DateTime date, int score)
  {
    StudyId = sutdyId;
    StackId = stackId;
    Date = date;
    Score = score;
  }
}