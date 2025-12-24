using Flashcards.TerrenceLGee.DTOs.SessionFlashcardDTOs;

namespace Flashcards.TerrenceLGee.DTOs.StudySessionDTOs;

public class RetrievedStudySessionDto
{
    public int Id { get; set; }
    public int StackId { get; set; }
    public int TotalQuestions { get; set; }
    public int Correct { get; set; }
    public int Incorrect { get; set; }
    public double Score { get; set; }
    public TimeSpan SessionDuration { get; set; }
    public List<SessionFlashcardDto> SessionFlashcards { get; set; } = [];
}