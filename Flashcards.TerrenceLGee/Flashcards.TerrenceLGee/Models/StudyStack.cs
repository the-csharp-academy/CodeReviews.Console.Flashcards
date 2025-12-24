namespace Flashcards.TerrenceLGee.Models;

public class StudyStack
{
    public int Id { get; set; }
    public Subject Subject { get; set; }
    public string Name { get; set; } = string.Empty;
    public List<Flashcard> Flashcards { get; set; } = [];
    public List<StudySession> StudySessions { get; set; } = [];
}