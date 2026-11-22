using Kerem.Flashcards.Domain.Entities;

namespace Kerem.Flashcards.Domain.Interfaces ;

    public interface IStudySessionRepository
    {
        string Create(StudySession studySession);
        List<StudySession> GetAllStudySessions();
    }