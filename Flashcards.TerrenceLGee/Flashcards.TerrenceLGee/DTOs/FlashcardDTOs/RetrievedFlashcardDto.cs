namespace Flashcards.TerrenceLGee.DTOs.FlashcardDTOs;

public class RetrievedFlashcardDto
{
    public int Id { get; set; }
    public int StackId { get; set; }
    public string Question { get; set; } = string.Empty;
    public string Answer { get; set; } = string.Empty;
    public int Position { get; set; }
}