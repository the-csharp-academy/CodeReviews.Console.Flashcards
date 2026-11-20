using Kerem.Flashcards.Domain.Entities;

namespace Kerem.Flashcards.Domain.Interfaces ;

    public interface IStudySessionRepository
    {
        void Study(StudySession studySession);
    }