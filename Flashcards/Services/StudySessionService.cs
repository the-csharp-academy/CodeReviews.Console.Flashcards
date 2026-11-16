using Flashcards.DTOs;
using Flashcards.Repositories;

namespace Flashcards.Services
{
    internal class StudySessionService
    {
        private StudySessionRepository _studySessionRepository;
        public StudySessionService(StudySessionRepository studySessionRepository)
        {
            _studySessionRepository = studySessionRepository;
        }

        internal async Task<List<StudySessionDTO>> CreateStudySessionDTOAsync(StackDTO? stack)
        {
            var studySessions = new List<Entities.StudySession>();
            if (stack == null)
            {
                studySessions = await _studySessionRepository.GetAllStudySessionsAsync();
            }
            else
            {
                studySessions = await _studySessionRepository.GetAllStudySessionsByStackAsync(stack);
            }

            return studySessions.Select((x, Index) => new StudySessionDTO
            {
                Id = x.Id,
                Number = Index + 1,
                StackId = x.StackId,
                StartTime = x.StartTime,
                Score = x.Score,
                EndTime = x.EndTime,
                Duration = x.Duration,
                Percentage = x.Percentage,
                TotalCards = x.TotalCards
            }).ToList();
        }

        internal async Task<StudySessionDTO> CreateStudySessionDTOFromInputAsync(int totalCards, int stackId, DateTime startTime, DateTime endTime, int correctAnswers)
        {
            return new StudySessionDTO
            {
                StackId = stackId,
                StartTime = startTime,
                Score = correctAnswers,
                EndTime = endTime,
                Duration = (Int32)(endTime - startTime).TotalMinutes,
                Percentage = Math.Round((decimal)correctAnswers / totalCards * 100, 2),
                TotalCards = totalCards
            };
        }

        internal async Task SaveStudySessionAsync(StudySessionDTO studySessionDTO)
        {
            await _studySessionRepository.SaveStudySessionAsync(studySessionDTO);
        }

    }
}
