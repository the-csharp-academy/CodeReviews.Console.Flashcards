public interface IFlashcardsRepo
{
    IReadOnlyList<Flashcard> GetAllByStackId(long stackId);
    void Add(Flashcard card);
    void Update(Flashcard card);
    void Delete(long cardId);

}