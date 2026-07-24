public interface IFlashcardsRepo
{
    IReadOnlyList<Flashcard> GetAllByStackId(int stackId);
    void Add(Flashcard card);
    void Update(Flashcard card);
    void Delete(int cardId);

}