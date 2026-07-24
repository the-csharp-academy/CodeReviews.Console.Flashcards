public class Flashcard
{
    public int FlashcardId;
    public int StackId;
    public string Question { get; set; } = string.Empty;
    public string Answer { get; set; } = string.Empty;
}

public class FlashcardDTO
{
    public int DisplayId { get; set; }
    public string Question { get; set; } = string.Empty;
    public string Answer { get; set; } = string.Empty;
}