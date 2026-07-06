namespace silvermax.FlashCards.Models;

internal class StudySession
{
    public int Id { get; set; }
    public string? Question { get; set; }
    public string? Answer { get; set; }
    public string? CorrectAnswer { get; set; }
}
