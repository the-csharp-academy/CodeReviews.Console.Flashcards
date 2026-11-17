namespace Kerem.Flashcards.Domain.Entities ;

    public class StackDTO
    {
        public string Name { get; set; }
        
        public ICollection<Flashcard> Flashcards { get; set; } = new List<Flashcard>();
    }