namespace Flashcards.DTOs
{
    public class GetFlashcardDTO
    {
        public int Id { get; set; }
        public int Number { get; set; }
        public string Front { get; set; }
        public string Back { get; set; }
    }
}
