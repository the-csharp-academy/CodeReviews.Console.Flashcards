namespace Flashcards.Interfaces
{
    public interface IMonthlyReport
    {
        int StackId { get; set; }
        string StackName { get; set; }
        public int Year { get; set; }
        IReadOnlyList<string> MonthlyValues { get; }
    }
}
