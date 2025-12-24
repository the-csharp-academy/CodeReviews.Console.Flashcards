namespace Flashcards.TerrenceLGee.Models;

public class Flashcard
{
    public int Id { get; set; }
    public int StackId { get; set; }
    public string Question { get; set; } = string.Empty;
    public string Answer { get; set; } = string.Empty;
    public int Position { get; set; }
}