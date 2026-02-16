namespace Flashcards.m1chael888.Models;

public class CardDto
{
    public int DisplayId { get; set; }
    public int CardId { get; set; }
    public string Front { get; set; } = string.Empty;
    public string Back { get; set; } = string.Empty;
}
