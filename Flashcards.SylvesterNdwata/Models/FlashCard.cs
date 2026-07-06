namespace silvermax.FlashCards.Models;

internal class FlashCard
{
    public int Id { get; set; }
    public int StackId { get; set; }
    public string? Word { get; set; }
    public string? Translation { get; set; }
}
