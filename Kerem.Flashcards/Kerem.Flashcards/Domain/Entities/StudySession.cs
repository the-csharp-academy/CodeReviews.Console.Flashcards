namespace Kerem.Flashcards.Domain.Entities ;

    public class StudySession
    {
        public int Id { get; set; }
        public DateTime StartedAt { get; set; }
        public int Scores{ get; set; }
        public int StackId  { get; set; }
        public Stack? Stack { get; set; }
    }