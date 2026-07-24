namespace CodeReviews.Console.Flashcards;

public interface IStudySessionsRepo
{
    void Add(StudySession session);
    IReadOnlyList<StudySessionDTO> GetAll();
}