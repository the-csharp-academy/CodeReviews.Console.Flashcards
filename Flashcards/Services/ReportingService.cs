using Flashcards.DTOs;
using Flashcards.Repositories;

namespace Flashcards.Services
{
    internal class ReportingService
    {
        private readonly ReportingRepository _reportingRepository;

        public ReportingService(ReportingRepository reportingRepository)
        {
            _reportingRepository = reportingRepository;
        }

        public async Task<List<MonthlySessionReportDTO>> GetMonthlyStudySessionsAsync(int year)
        {
            return await _reportingRepository.ShowMonthlyStudySessionsByYearAsync(year);
        }

        public async Task<List<MonthlyAvgScoreReportDTO>> GetMonthlyStudyAvgAsync(int year)
        {
            return await _reportingRepository.ShowMonthlySessionsAvgByYearAsync(year);
        }

    }
}
