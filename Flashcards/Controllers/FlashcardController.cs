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
        if (!TryGetCurrentStack(out CardStack currentStack))
            return;

        var (que, ans) = _flashcardsView.AskFlashcardContent();

        if (!RepositoryHelpers.TryExecute(
            () => _flashcardsRepo.Add(
                new Flashcard
                {
                    StackId = currentStack.StackId,
                    Question = que,
                    Answer = ans
                }),
                out Exception? ex
        ))
        {
            _flashcardsView.DisplayError($"Could not add flashcard: {ex!.Message}");
            return;
        }
        _flashcardsView.DisplayMessage("Flashcard added successfully.");
    }
    public string ChangeStack() => _flashcardsView.SelectStack();
    public void DeleteCard()
    {
        if (!TryGetCurrentStack(out CardStack currentStack))
            return;

        if (!TryGetCards(currentStack.StackId, out IReadOnlyList<Flashcard> cards))
            return;

        if (cards.Count == 0)
        {
            _flashcardsView.DisplayMessage("No flashcards found to delete.");
            _flashcardsView.WaitForInput();
            return;
        }

        if (!TrySelectCard(cards, out Flashcard selectedCard))
            return;

        int cardId = selectedCard.FlashcardId;

        if (!RepositoryHelpers.TryExecute(
            () => _flashcardsRepo.Delete(cardId),
            out Exception? ex))
        {
            _flashcardsView.DisplayError($"Failed to delete flashcard: {ex!.Message}");
            return;
        }

        _flashcardsView.DisplayMessage("Flashcard deleted successfully.");
    }

    public void EditCard()
    {
        if (!TryGetCurrentStack(out CardStack currentStack))
            return;

        if (!TryGetCards(currentStack.StackId, out IReadOnlyList<Flashcard> cards))
            return;

        if (cards.Count == 0)
        {
            _flashcardsView.DisplayMessage("No flashcards found to edit.");
            _flashcardsView.WaitForInput();
            return;
        }

        if (!TrySelectCard(cards, out Flashcard selectedCard))
            return;

        var (question, answer) = _flashcardsView.AskFlashcardContent();
        selectedCard.Question = question;
        selectedCard.Answer = answer;

        if (!RepositoryHelpers.TryExecute(
            () => _flashcardsRepo.Update(selectedCard),
            out Exception? ex))
        {
            _flashcardsView.DisplayError($"Failed to update flashcard: {ex!.Message}");
            return;
        }
        _flashcardsView.DisplayMessage("Flashcard updated successfully.");

    }

    public void ViewCards()
    {
        if (!TryGetCurrentStack(out CardStack currentStack))
            return;

        if (!TryGetCards(currentStack.StackId, out IReadOnlyList<Flashcard> cards))
            return;

        if (cards.Count == 0)
        {
            _flashcardsView.DisplayMessage("No flashcards found.");
            _flashcardsView.WaitForInput();
            return;
        }
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

    private bool TryGetCards(int stackId, out IReadOnlyList<Flashcard> cards)
    {
        cards = Array.Empty<Flashcard>();
        try
        {
            cards = _flashcardsRepo.GetAllByStackId(stackId);
            return true;
        }
        catch (Exception ex)
        {
            _flashcardsView.DisplayError($"An error occurred while retrieving flashcards: {ex.Message}");
            return false;
        }
    }
    private bool TryGetCurrentStack(out CardStack currentStack)
    {
        currentStack = null!;

        if (string.IsNullOrWhiteSpace(_currentStackName))
        {
            _flashcardsView.DisplayError("Select a stack first.");
            return false;
        }

        try
        {
            CardStack? foundStack = _stacksRepo.GetStackByName(_currentStackName);

            if (foundStack is null)
            {
                _flashcardsView.DisplayError($"Stack '{_currentStackName}' could not be found.");
                return false;
            }

            currentStack = foundStack;
            return true;
        }
        catch (Exception ex)
        {
            _flashcardsView.DisplayError($"An error occurred while retrieving the stack: {ex.Message}");
            return false;
        }
    }

    private bool TrySelectCard(IReadOnlyList<Flashcard> cards, out Flashcard selectedCard)
    {
        selectedCard = null!;
        MapCardsToDTO(cards, out List<FlashcardDTO> dto);

        _flashcardsView.DisplayFlashcards(dto);
        try
        {
            int selectedIndex = _flashcardsView.AskFlashcardIndex(dto.Count);
            selectedCard = cards[selectedIndex - 1];
            return true;
        }
        catch (Exception ex)
        {
            _flashcardsView.DisplayError($"Invalid selection: {ex.Message}");
            return false;
        }
    }
}