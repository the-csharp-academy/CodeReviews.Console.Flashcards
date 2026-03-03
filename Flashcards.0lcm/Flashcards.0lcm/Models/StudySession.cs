namespace Flashcards._0lcm.Models;

public class StudySession
{
    public int SessionId { get; set; }
    public int StudyCount { get; set; }
    public DateTime Date { get; set; } = DateTime.Today;
    public int StackId { get; set; }
}