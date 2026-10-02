namespace Flashcards.Entities
{
    public class FlashcardEntity
    {
        public int Id { get; set; }
        public int CardStackId { get; set; }
        public string Front { get; set; }
        public string Back { get; set; }
    }
}
