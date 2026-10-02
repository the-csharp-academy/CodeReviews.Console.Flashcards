using Flashcards.DTOs;
using Flashcards.Entities;
using Flashcards.Repositories;
using Microsoft.Extensions.Configuration;

namespace Flashcards.Controllers
{
    public class StudySessionController
    {
        private readonly StudySessionRepository repository;

        public StudySessionController()
        {
            var configuration = new ConfigurationBuilder()
                .SetBasePath(AppContext.BaseDirectory)
                .AddJsonFile("appsettings.json", optional: false)
                .Build();

            var connectionString = configuration.GetConnectionString("DefaultConnection");
            repository = new(connectionString);
            repository.CreateTable();
        }

        public void CreateSession(CreateStudySessionDTO studySession)
        {
            var entity = new StudySessionEntity
            {
                CardStackId = studySession.CardStackId,
                Time = studySession.Time,
                Score = studySession.Score
            };
            repository.CreateSession(entity);
        }

        public List<GetStudySessionDTO> ReadSessions(List<CardStackDTO> cardStacks)
        {
            var sessions = repository.ReadSessions();
            return sessions
                .Join(
                cardStacks,
                session => session.CardStackId,
                cardStack => cardStack.Id,
                (session, cardStack) => new GetStudySessionDTO
                {
                    CardStackName = cardStack.Name,
                    Time = session.Time,
                    Score = session.Score
                })
                .ToList();
        }

        public List<MonthlySessionDTO> GetMonthlySessions(int year)
        {
            return  repository.GetMonthlySessions(year);
        }

        public List<AverageScoreDTO> GetAverageScores(int month , int year)
        {
            return repository.GetAverageScores(month, year);
        }
    }
}
