namespace Kerem.Flashcards.Domain.Entities;

    public class Flashcard
    {
        public int Id { get; set; }
        public string? Name { get; set; }
        public int StackId { get; set; }
        public Stack? Stack { get; set; }

    }