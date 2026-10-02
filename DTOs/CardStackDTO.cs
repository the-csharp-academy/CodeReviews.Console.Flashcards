namespace Flashcards.DTOs
{
    public class CardStackDTO
    {
        public int Id { get; set; }
        public string Name { get; set; }

        public override string ToString()
        {
            return Name;
        }
    }
}
