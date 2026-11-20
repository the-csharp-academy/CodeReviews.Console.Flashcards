using Kerem.Flashcards.Domain.Entities;
using Kerem.Flashcards.Domain.Interfaces;
using Kerem.Flashcards.Infrastructure.Persistence;

namespace Kerem.Flashcards.Infrastructure.Repositories ;

    public class StudySessionRepository : IStudySessionRepository
    {
        private readonly DapperDbContext context;
        public StudySessionRepository(DapperDbContext context)
        {
            this.context = context;
        }

        public void Study(StudySession studySession)
        {
            
        }
        
        
    }