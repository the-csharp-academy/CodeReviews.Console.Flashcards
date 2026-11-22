namespace Kerem.Flashcards.Domain.Entities ;

    public class StackDTO
    {
        public string? Name { get; set; }
        
        public ICollection<FlashcardDto> Flashcards { get; set; } = new List<FlashcardDto>();
    }