namespace Flashcards._0lcm.Models;

public class StudySession
{
    public int SessionId { get; set; } = 0;
    public int StudyCount { get; set; } = 0;
    public DateTime Date  { get; set; } = DateTime.Today;
    public int StackId { get; set; } = 0;
}