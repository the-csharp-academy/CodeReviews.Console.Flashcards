using Flashcards.Interfaces;

namespace Flashcards.DTOs
{
    public class MonthlyAvgScoreReportDTO : IMonthlyReport
    {
        public int StackId { get; set; }
        public string StackName { get; set; }
        public int Year { get; set; }
        public decimal January { get; set; }
        public decimal February { get; set; }
        public decimal March { get; set; }
        public decimal April { get; set; }
        public decimal May { get; set; }
        public decimal June { get; set; }
        public decimal July { get; set; }
        public decimal August { get; set; }
        public decimal September { get; set; }
        public decimal October { get; set; }
        public decimal November { get; set; }
        public decimal December { get; set; }

        public IReadOnlyList<string> MonthlyValues =>
        [
            January.ToString("0.##"),
            February.ToString("0.##"),
            March.ToString("0.##"),
            April.ToString("0.##"),
            May.ToString("0.##"),
            June.ToString("0.##"),
            July.ToString("0.##"),
            August.ToString("0.##"),
            September.ToString("0.##"),
            October.ToString("0.##"),
            November.ToString("0.##"),
            December.ToString("0.##")
        ];
    }
}
