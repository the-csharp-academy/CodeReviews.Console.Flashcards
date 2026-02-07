using Flashcards.m1chael888.Repositories;
using Flashcards.m1chael888.Models;

namespace Flashcards.m1chael888.Services
{
    public interface IStudyService
    {
        void SessionCreate(SessionModel session);
        public List<SessionModel> SessionsRead();
    }
    public class StudyService : IStudyService
    {
        private ISessionRepository _sessionRepository;
        public StudyService(ISessionRepository sessionRepository) 
        {
            _sessionRepository = sessionRepository;
        }

        public void SessionCreate(SessionModel session)
        {
            _sessionRepository.Create(session);
        }

        public List<SessionModel> SessionsRead()
        {
            var sessions = _sessionRepository.Read();
            return sessions;
        }
    }
}
