using static Flashcards.m1chael888.Enums.ManageEnums;
using Flashcards.m1chael888.Services;
using Flashcards.m1chael888.Models;
using Flashcards.m1chael888.Views;
using Spectre.Console;

namespace Flashcards.m1chael888.Controllers;

public class ManageController
{
    private readonly IStackView _stackView;
    private readonly ICardView _cardView;
    private readonly IStackService _stackService;
    private readonly ICardService _cardService;
    public ManageController(IStackView stackView, ICardView cardView, IStackService stackService, ICardService cardService)
    {
        _stackView = stackView;
        _cardView = cardView;
        _stackService = stackService;
        _cardService = cardService;
    }
    public void HandleManageMenu()
    {
        Console.Clear();
        var choice = CallManageMenu();

        switch (choice)
        {
            case StackMenuOption.CreateStack:
                CallStackCreate();
                break;
            case StackMenuOption.ViewStacks:
                CallStacksRead();
                break;
            case StackMenuOption.UpdateStack:
                CallStackUpdate();
                break;
            case StackMenuOption.DeleteStack:
                CallStackDelete();
                break;
            case StackMenuOption.Back:
                break;
        }
    }

    private StackMenuOption CallManageMenu()
    {
        Console.Clear();
        var choice = _stackView.ShowMenu();
        return choice;
    }

    private void CallStackCreate()
    {
        string stackName = CallGetStackName("Create stack::");

        _stackService.StackCreate(stackName);
        ReturnToManageMenu("Stack saved successfully");
    }

    private void CallStacksRead()
    {
        Console.Clear();
        var stacks = GetStackList();
        var choice = _stackView.DisplayStackList(stacks);
        
        switch (choice)
        {
            case ViewStacksOption.ViewCards:
                CallCardsRead();
                break;
            case ViewStacksOption.Back:
                HandleManageMenu();
                break;
        }
    }

    private void CallStackUpdate()
    {
        var stacks = GetStackList();
        var choice = _stackView.DisplayStackPrompt(stacks, "Choose a stack of cards to update::");
        choice.Name = CallGetStackName("Update stack::");

        _stackService.StackUpdate(choice);

        ReturnToManageMenu("Stack updated successfully");
    }

    private void CallStackDelete()
    {
        var stacks = GetStackList();
        var choice = _stackView.DisplayStackPrompt(stacks, "Choose a stack of cards to delete::");
        _stackService.StackDelete(choice);

        ReturnToManageMenu("Stack deleted successfully");
    }

    private void CallShowCards(List<CardDto> cards, StackModel choice)
    {
        Console.Clear();
        if (cards.Count > 0)
        {
            _cardView.DisplayCardList(cards, choice.Name);
        }
        else
        {
            AnsiConsole.MarkupLine("[lime]This stack is empty!! Create some cards[/]\n");
        }
        HandleCardsReadMenu(choice);
    }

    private void HandleCardsReadMenu(StackModel choice)
    {
        switch (_cardView.DisplayCardMenu())
        {
            case ViewCardsOption.CreateCard:
                CallCardCreate(choice);
                break;
            case ViewCardsOption.UpdateCard:
                CallCardUpdate(choice);
                break;
            case ViewCardsOption.DeleteCard:
                CallCardDelete(choice);
                break;
            case ViewCardsOption.Back:
                CallStacksRead();
                break;
        }
    }

    void CallCardCreate(StackModel choice)
    {
        var card = new CardModel();
        var front = _cardView.GetCardFront("Creating");
        var back = _cardView.GetCardBack("Creating");
        card.Front = front;
        card.Back = back;
        card.StackId = choice.StackId;

        _cardService.CardCreate(card);
        ReturnToCardList(choice, "Card created successfully =)");
    }

    private void CallCardsRead()
    {
        Console.Clear();
        var stacks = GetStackList();
        var choice = _stackView.DisplayStackPrompt(stacks, "Choose which stack's cards youd like to view::");
        var cards = _cardService.CardsRead(choice);

        CallShowCards(cards, choice);
    }

    void CallCardUpdate(StackModel choice)
    {
        var cards = GetCardList(choice);
        if (cards.Count == 0)
        {
            CallShowCards(cards, choice);
        }
        else
        {
            var card = _cardView.DisplayCardPrompt(cards, "Choose a card to update::");
            var front = _cardView.GetCardFront("Updating");
            var back = _cardView.GetCardBack("Updating");
            var updatedCard = new CardModel();
            updatedCard.CardId = card.CardId;
            updatedCard.Front = front;
            updatedCard.Back = back;
            updatedCard.StackId = choice.StackId;

            _cardService.CardUpdate(updatedCard);
            ReturnToCardList(choice, "Card updated successfully =)");
        }
    }

    void CallCardDelete(StackModel choice)
    {
        var cards = GetCardList(choice);
        if (cards.Count == 0)
        {
            CallShowCards(cards, choice);
        }
        else
        {
            var card = _cardView.DisplayCardPrompt(cards, "Choose a card to delete::");
            _cardService.CardDelete(card.CardId);
            ReturnToCardList(choice, "Card deleted successfully =)");
        }
    }

    private List<StackModel> GetStackList()
    {
        var stacks = _stackService.StacksRead();
        return stacks;
    }
     
    private List<CardDto> GetCardList(StackModel choice)
    {
        var cards = _cardService.CardsRead(choice);
        return cards;
    }

    private string CallGetStackName(string msg)
    {
        var stacks = GetStackList();
        string stackName = _stackView.GetStackName(msg);

        while (stacks.Where(x => x.Name == stackName).Any())
        {
            stackName = _stackView.GetStackName(msg, exists: true);
        }
        return stackName;
    }

    private void ReturnToManageMenu(string msg)
    {
        Console.Clear();    
        AnsiConsole.MarkupLine($"[lime]{msg}[/]");

        AnsiConsole.Status()
            .Spinner(Spinner.Known.Point)
            .SpinnerStyle("white")
            .Start("Press any key to return to menu", x =>
            {
                Console.ReadKey();
            });
        HandleManageMenu();
    }

    private void ReturnToCardList(StackModel choice, string msg)
    {
        Console.Clear();
        AnsiConsole.MarkupLine($"[lime]{msg}[/]");

        AnsiConsole.Status()
            .Spinner(Spinner.Known.Point)
            .SpinnerStyle("white")
            .Start("Press any key to return", x =>
            {
                Console.ReadKey();
            });
        CallShowCards(GetCardList(choice), choice);
    }
}
