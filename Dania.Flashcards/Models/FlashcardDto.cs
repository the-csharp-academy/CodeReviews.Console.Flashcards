namespace Flashcards.Models
{
    internal class FlashcardDto
    {
        public int Id { get; set; }
        public int DisplayId { get; set; }
        public string Question { get; set; }
        public string Answer { get; set; }
    }
}
