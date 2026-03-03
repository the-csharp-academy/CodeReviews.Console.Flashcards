namespace Flashcards._0lcm.Models;

public class Flashcard
{
    public int CardId { get; set; }
    public string Name { get; set; } = string.Empty;
    public string Value { get; set; } = string.Empty;
    public int StackId { get; set; }
}