using Flashcards._0lcm.DTOs;

namespace Flashcards._0lcm.Interfaces;

public interface IFlashcardService
{
    void CreateFlashcard(string name, string value, int stackId);
    void UpdateFlashcard(int cardId, string? name, string? value);
    void DeleteFlashcard(int cardId);
    List<FlashcardDto> GetFlashcardDtosForStack(StackDto stackDto);
}