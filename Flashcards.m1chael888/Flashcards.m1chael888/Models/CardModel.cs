namespace Flashcards.m1chael888.Models;

public class CardModel
{
    public int CardId { get; set; }
    public string Front { get; set; } = string.Empty;
    public string Back { get; set; } = string.Empty;
    public int StackId { get; set; }
}
