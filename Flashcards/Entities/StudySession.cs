namespace Flashcards.Entities
{
    public class StudySession
    {
        public int? Id { get; set; }
        public int StackId { get; set; }
        public int TotalCards { get; set; }
        public int Score { get; set; }
        public decimal Percentage { get; set; }
        public DateTime StartTime { get; set; }
        public DateTime EndTime { get; set; }
        public int Duration { get; set; }
    }
}
