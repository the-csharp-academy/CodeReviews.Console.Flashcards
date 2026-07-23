public class Flashcard
{
    public long FlashcardId;
    public long StackId;
    public string Question { get; set; } = string.Empty;
    public string Answer { get; set; } = string.Empty;
}

public class FlashcardDTO
{
    public long DisplayId { get; set; }
    public string Question { get; set; } = string.Empty;
    public string Answer { get; set; } = string.Empty;
}