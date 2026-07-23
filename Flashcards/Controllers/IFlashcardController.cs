public interface IFlashcardController
{
    void Run();
    string ChangeStack();
    void ViewCards();
    void AddCard();
    void EditCard();
    void DeleteCard();
}