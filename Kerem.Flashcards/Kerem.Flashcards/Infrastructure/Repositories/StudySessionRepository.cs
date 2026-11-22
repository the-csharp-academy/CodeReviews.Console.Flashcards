using Dapper;
using Kerem.Flashcards.Domain.Entities;
using Kerem.Flashcards.Domain.Interfaces;
using Kerem.Flashcards.Infrastructure.Persistence;

namespace Kerem.Flashcards.Infrastructure.Repositories ;

    public class StudySessionRepository : IStudySessionRepository
    {
        private readonly DapperDbContext _dapperDbContext;
        public StudySessionRepository(DapperDbContext context)
        {
            this._dapperDbContext = context;
        }

        public string Create(StudySession studySession)
        {
            var connection = _dapperDbContext.GetConnection();
            var sql = "INSERT INTO StudySessions(createdat, score, stackid) VALUES (@CreatedAt, @Score, @Stackid)";
            connection.Execute(sql, studySession);
            return "Study Session created!";
        }
        
        public List<StudySession> GetAllStudySessions()
        {
            var connection = _dapperDbContext.GetConnection();
            var sql = "SELECT * FROM Studysessions";
            List<StudySession> studySessions = connection.Query<StudySession>(sql).ToList();
            return studySessions;
        }
        
        
    }