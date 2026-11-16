using Flashcards.Interfaces;

namespace Flashcards.DTOs
{
    public class MonthlySessionReportDTO : IMonthlyReport
    {
        public int StackId { get; set; }
        public string StackName { get; set; }
        public int Year { get; set; }
        public int January { get; set; }
        public int February { get; set; }
        public int March { get; set; }
        public int April { get; set; }
        public int May { get; set; }
        public int June { get; set; }
        public int July { get; set; }
        public int August { get; set; }
        public int September { get; set; }
        public int October { get; set; }
        public int November { get; set; }
        public int December { get; set; }

        public IReadOnlyList<string> MonthlyValues =>
       [
           January.ToString(),
            February.ToString(),
            March.ToString(),
            April.ToString(),
            May.ToString(),
            June.ToString(),
            July.ToString(),
            August.ToString(),
            September.ToString(),
            October.ToString(),
            November.ToString(),
            December.ToString()
       ];
    }
}
