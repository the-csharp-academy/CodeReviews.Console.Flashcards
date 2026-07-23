public interface IFlashcardsView
{
    void DisplayMessage(string message);
    void DisplayError(string message);
    FlashcardsOption ShowFlashcardsOption();
    void DisplayFlashcards(IReadOnlyList<FlashcardDTO> cards);
    string SelectStack();
    (string question, string answer) AskFlashcardContent();
    int AskFlashcardIndex(int maxIndex);
    void WaitForInput();
}