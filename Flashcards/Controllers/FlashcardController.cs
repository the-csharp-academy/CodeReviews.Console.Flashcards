public sealed class FlashcardController : IFlashcardController
{
    private readonly IFlashcardsView _flashcardsView;
    private readonly IFlashcardsRepo _flashcardsRepo;
    private readonly IStacksRepo _stacksRepo;
    private string? _currentStackName;
    public FlashcardController(IFlashcardsView view, IFlashcardsRepo cardRepo, IStacksRepo stackRepo)
    {
        _flashcardsView = view;
        _flashcardsRepo = cardRepo;
        _stacksRepo = stackRepo;
    }
    public void Run()
    {
        bool isRunning = true;
        while (isRunning)
        {
            _flashcardsView.DisplayMessage($"Current stack name: {_currentStackName ?? "-"}");
            FlashcardsOption selectedOption = _flashcardsView.ShowFlashcardsOption();
            switch (selectedOption)
            {
                case FlashcardsOption.ChangeStack:
                    _currentStackName = ChangeStack();
                    break;
                case FlashcardsOption.ViewFlashcards:
                    ViewCards();
                    break;
                case FlashcardsOption.AddFlashcards:
                    AddCard();
                    break;
                case FlashcardsOption.EditFlashcards:
                    EditCard();
                    break;
                case FlashcardsOption.DeleteFlashcards:
                    DeleteCard();
                    break;
                case FlashcardsOption.Back:
                    isRunning = false;
                    break;
                default:
                    throw new ArgumentOutOfRangeException(
                        nameof(selectedOption),
                        selectedOption,
                        "Unknown menu option.");
            }
        }
    }
    public void AddCard()
    {
        if (string.IsNullOrWhiteSpace(_currentStackName))
        {
            _flashcardsView.DisplayError("Select a stack first.");
            return;
        }

        CardStack? currentStack;
        try
        {
            currentStack = _stacksRepo.GetStackByName(_currentStackName);
        }
        catch (Exception ex)
        {
            _flashcardsView.DisplayError($"An error occurred while retrieving the stack: {ex.Message}");
            return;
        }

        if (currentStack == null)
        {
            _flashcardsView.DisplayError($"Stack '{_currentStackName}' could not be found.");
            return;
        }

        var (que, ans) = _flashcardsView.AskFlashcardContent();
        _flashcardsRepo.Add(new Flashcard
        {
            StackId = currentStack.StackId,
            Question = que,
            Answer = ans
        });
    }

    public string ChangeStack() => _flashcardsView.SelectStack();
    public void DeleteCard()
    {
        if (string.IsNullOrWhiteSpace(_currentStackName))
        {
            _flashcardsView.DisplayError("Select a stack first.");
            return;
        }

        CardStack? currentStack;
        try
        {
            currentStack = _stacksRepo.GetStackByName(_currentStackName);
        }
        catch (Exception ex)
        {
            _flashcardsView.DisplayError($"An error occurred while retrieving the stack: {ex.Message}");
            return;
        }

        if (currentStack == null)
        {
            _flashcardsView.DisplayError($"Stack '{_currentStackName}' could not be found.");
            return;
        }

        IReadOnlyList<Flashcard> cards = _flashcardsRepo.GetAllByStackId(currentStack.StackId);
        if (cards.Count == 0)
        {
            _flashcardsView.DisplayMessage("No flashcards found to delete.");
            _flashcardsView.WaitForInput();
            return;
        }

        MapCardsToDTO(cards, out List<FlashcardDTO> dto);
        _flashcardsView.DisplayFlashcards(dto);

        int selectedIndex;
        try
        {
            selectedIndex = _flashcardsView.AskFlashcardIndex(dto.Count);
        }
        catch (Exception ex)
        {
            _flashcardsView.DisplayError($"Invalid selection: {ex.Message}");
            return;
        }

        long cardId = cards[selectedIndex - 1].FlashcardId;

        try
        {
            _flashcardsRepo.Delete(cardId);
            _flashcardsView.DisplayMessage("Flashcard deleted successfully.");
        }
        catch (Exception ex)
        {
            _flashcardsView.DisplayError($"Failed to delete flashcard: {ex.Message}");
        }
    }

    public void EditCard()
    {
        if (string.IsNullOrWhiteSpace(_currentStackName))
        {
            _flashcardsView.DisplayError("Select a stack first.");
            return;
        }

        CardStack? currentStack;
        try
        {
            currentStack = _stacksRepo.GetStackByName(_currentStackName);
        }
        catch (Exception ex)
        {
            _flashcardsView.DisplayError($"An error occurred while retrieving the stack: {ex.Message}");
            return;
        }

        if (currentStack == null)
        {
            _flashcardsView.DisplayError($"Stack '{_currentStackName}' could not be found.");
            return;
        }

        IReadOnlyList<Flashcard> cards = _flashcardsRepo.GetAllByStackId(currentStack.StackId);
        if (cards.Count == 0)
        {
            _flashcardsView.DisplayMessage("No flashcards found to edit.");
            _flashcardsView.WaitForInput();
            return;
        }

        MapCardsToDTO(cards, out List<FlashcardDTO> dto);
        _flashcardsView.DisplayFlashcards(dto);

        int selectedIndex;
        try
        {
            selectedIndex = _flashcardsView.AskFlashcardIndex(dto.Count);
        }
        catch (Exception ex)
        {
            _flashcardsView.DisplayError($"Invalid selection: {ex.Message}");
            return;
        }

        Flashcard selectedCard = cards[selectedIndex - 1];
        var (question, answer) = _flashcardsView.AskFlashcardContent();
        selectedCard.Question = question;
        selectedCard.Answer = answer;

        try
        {
            _flashcardsRepo.Update(selectedCard);
            _flashcardsView.DisplayMessage("Flashcard updated successfully.");
        }
        catch (Exception ex)
        {
            _flashcardsView.DisplayError($"Failed to update flashcard: {ex.Message}");
        }
    }

    public void ViewCards()
    {
        if (string.IsNullOrWhiteSpace(_currentStackName))
        {
            _flashcardsView.DisplayError("Select a stack first.");
            return;
        }

        CardStack? currentStack;
        try
        {
            currentStack = _stacksRepo.GetStackByName(_currentStackName);
        }
        catch (Exception ex)
        {
            _flashcardsView.DisplayError($"An error occurred while retrieving the stack: {ex.Message}");
            return;
        }

        if (currentStack == null)
        {
            _flashcardsView.DisplayError($"Stack '{_currentStackName}' could not be found.");
            return;
        }

        IReadOnlyList<Flashcard> cards = _flashcardsRepo.GetAllByStackId(currentStack.StackId);
        MapCardsToDTO(cards, out List<FlashcardDTO> dto);

        _flashcardsView.DisplayFlashcards(dto);
        _flashcardsView.WaitForInput();
    }

    private void MapCardsToDTO(IReadOnlyList<Flashcard> cards, out List<FlashcardDTO> dto)
    {
        dto = new List<FlashcardDTO>();
        int counter = 0;
        foreach (var item in cards)
        {
            dto.Add(new FlashcardDTO
            {
                DisplayId = ++counter,
                Question = item.Question,
                Answer = item.Answer
            });
        }
    }

}