namespace Flashcards;

internal class Stack
{
  internal int StackId { get; private set; }
  internal string Name { get; private set; }

  public Stack(int stackId, string name)
  {
    StackId = stackId;
    Name = name;
  }

  internal void EditName(string name)
  {
    Name = name;
  }
}

