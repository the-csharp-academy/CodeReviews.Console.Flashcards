namespace Flashcards.DTOs
{
    public class FlashcardDTO
    {
        public int Id { get; set; }
        public int Number { get; set; }
        public int StackId { get; set; }
        public string Front { get; set; }
        public string Back { get; set; }
    }
}
