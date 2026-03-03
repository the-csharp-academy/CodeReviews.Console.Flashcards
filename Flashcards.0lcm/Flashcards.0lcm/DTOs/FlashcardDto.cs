namespace Flashcards._0lcm.DTOs;

public class FlashcardDto
{
    public int CardId { get; set; }
    public int DisplayId { get; set; }
    public string Name { get; set; } = string.Empty;
    public string Value { get; set; } = string.Empty;
    public string StackName { get; set; } = string.Empty;
}