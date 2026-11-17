namespace Kerem.Flashcards.Domain.Entities ;

    public class Stack
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public ICollection<Flashcard> Flashcards { get; set; } = new List<Flashcard>();
    }