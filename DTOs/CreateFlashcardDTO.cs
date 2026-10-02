namespace Flashcards.DTOs
{
    public class CreateFlashcardDTO
    {
        public int Id { get; set; }
        public int CardStackId { get; set; }
        public string Front {  get; set; }
        public string Back { get; set; }
        public override string ToString()
        {
            return Front + " - " + Back;
        }
    }
}
