namespace Flashcards;

internal class Flashcard
{
  internal int FlashcardId { get; private set; }
  internal int StackId { get; private set; }
  internal string Recto { get; private set; }
  internal string Verso { get; private set; }
  internal string Display => $"{Recto} -> {Verso}";

  internal Flashcard(int flashcardId, string recto, string verso, int stackId)
  {
    FlashcardId = flashcardId;
    StackId = stackId;
    Recto = recto;
    Verso = verso;
  }

  internal void EditRecto(string recto)
  {
    Recto = recto;
  }

  internal void EditVerso(string verso)
  {
    Verso = verso;
  }
}