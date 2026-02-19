namespace Flashcards._0lcm.DTOs;

public class FlashcardDto
{
    public int CardId { get; set; } = 0;
    public int DisplayId { get; set; } = 0;
    public string Name { get; set; } = string.Empty;
    public string Value { get; set; } = string.Empty;
    public string StackName { get; set; } = string.Empty;
}