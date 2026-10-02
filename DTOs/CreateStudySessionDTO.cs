namespace Flashcards.DTOs
{
    public class CreateStudySessionDTO
    {
        public int CardStackId { get; set; }
        public DateTime Time { get; set; }
        public int Score { get; set; }
    }
}
