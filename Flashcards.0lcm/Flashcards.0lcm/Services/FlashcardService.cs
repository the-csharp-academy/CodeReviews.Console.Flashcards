using Flashcards._0lcm.CRUDController;
using Flashcards._0lcm.DTOs;
using Flashcards._0lcm.Interfaces;
using Flashcards._0lcm.Models;

namespace Flashcards._0lcm.Services;

public class FlashcardService : IFlashcardService
{
    public void CreateFlashcard(string name, string value, int stackId)
    {
        var flashcard = new Flashcard
        {
            Name = name,
            Value = value,
            StackId = stackId
        };

        LocalDbController.CreateFlashcard(flashcard);
    }

    public void UpdateFlashcard(int cardId, string? name, string? value)
    {
        if (string.IsNullOrEmpty(name) || string.IsNullOrEmpty(value))
            throw new ArgumentNullException("Name or Value parameter is/are null or empty.");

        var flashcard = new Flashcard
        {
            CardId = cardId,
            Name = name,
            Value = value
        };

        LocalDbController.UpdateFlashcard(flashcard);
    }

    public void DeleteFlashcard(int cardId)
    {
        var flashcard = new Flashcard
        {
            CardId = cardId
        };
        LocalDbController.DeleteFlashcard(flashcard);
    }

    public List<FlashcardDto> GetFlashcardDtosForStack(StackDto dto)
    {
        var flashcards = LocalDbController.GetFlashcardsForStack(dto.StackId)
            .OrderBy(f => f.Name);

        var flashcardDtos = new List<FlashcardDto>();
        var displayId = 1;

        foreach (var flashcard in flashcards)
            flashcardDtos.Add(new FlashcardDto
            {
                CardId = flashcard.CardId,
                DisplayId = displayId++,
                Name = flashcard.Name,
                Value = flashcard.Value,
                StackName = dto.Name
            });

        return flashcardDtos;
    }
}