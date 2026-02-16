using Flashcards.m1chael888.Models;
using Flashcards.m1chael888.Repositories;

namespace Flashcards.m1chael888.Services;

public interface ICardService
{
    void CardCreate(CardModel card);
    List<CardDto> CardsRead(StackModel stack);
    void CardUpdate(CardModel card);
    void CardDelete(int cardId);
}
public class CardService : ICardService
{
    private readonly ICardRepository _cardRepository;
    public CardService(ICardRepository cardRepository)
    {
        _cardRepository = cardRepository;
    }

    public void CardCreate(CardModel card)
    {
        _cardRepository.Create(card);
    }

    public List<CardDto> CardsRead(StackModel stack)
    {
        List<CardModel> cardModels = _cardRepository.Read(stack.StackId);
        var cardDtos = new List<CardDto>();

        for (int i = 0; i < cardModels.Count(); i++)
        {
            var cardDto = new CardDto();
            cardDto.DisplayId = i + 1;
            cardDto.CardId = cardModels[i].CardId;
            cardDto.Front = cardModels[i].Front;
            cardDto.Back = cardModels[i].Back;
            cardDtos.Add(cardDto);
        }
        return cardDtos;
    }

    public void CardUpdate(CardModel card)
    {
        _cardRepository.Update(card);
    }

    public void CardDelete(int cardId)
    {
        _cardRepository.Delete(cardId);
    }
}
