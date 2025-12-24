namespace Flashcards.TerrenceLGee.Models;

public class SessionFlashcard
{
    public int SessionId { get; set; }
    public int FlashcardId { get; set; }
    public string Question { get; set; } = string.Empty;
    public string Answer { get; set; } = string.Empty;
    public string UserAnswer { get; set; } = string.Empty;
    public bool IsCorrect { get; set; }
    public int DisplayPosition { get; set; }
}