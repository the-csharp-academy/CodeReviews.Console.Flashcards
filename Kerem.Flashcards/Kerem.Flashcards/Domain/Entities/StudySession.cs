namespace Kerem.Flashcards.Domain.Entities ;

    public class StudySession
    {
        public int Id { get; set; }
        public DateTime CreatedAt { get; set; }
        public int Score{ get; set; }
        public int StackId  { get; set; }
        public Stack? Stack { get; set; }
    }