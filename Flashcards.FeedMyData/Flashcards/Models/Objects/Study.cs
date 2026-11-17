using System.Dynamic;

namespace Flashcards;

internal class Study
{
  internal int StudyId { get; private set; }
  internal int StackId { get; private set; }
  internal DateTime Date { get; private set; }
  internal int Score { get; private set; }
  internal int StackSize { get; private set; }

  internal Study(int studyId, int stackId, DateTime date, int score, int stackSize)
  {
    StudyId = studyId;
    StackId = stackId;
    Date = date;
    Score = score;
    StackSize = stackSize;
  }
}