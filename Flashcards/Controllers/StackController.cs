public sealed class StackController : IStackController
{
    private readonly IStacksView _stacksView;
    private readonly IStacksRepo _stacksRepo;
    public StackController(IStacksView view, IStacksRepo repo)
    {
        _stacksView = view;
        _stacksRepo = repo;
    }

    public void Run()
    {
        bool isRunning = true;
        while (isRunning)
        {
            StacksOption selectedOption = _stacksView.ShowStacksOption();
            switch (selectedOption)
            {
                case StacksOption.ViewStacks:
                    ViewStacks();
                    break;
                case StacksOption.AddStacks:
                    AddStack();
                    break;
                case StacksOption.EditStacks:
                    EditStack();
                    break;
                case StacksOption.DeleteStacks:
                    DeleteStack();
                    break;
                case StacksOption.Back:
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
    public void AddStack()
    {
        try
        {
            _stacksRepo.Add(_stacksView.AskForStackName());
        }
        catch (ArgumentNullException e)
        {
            _stacksView.DisplayError("Stack name cannot be null" + e.Message);
        }
    }

    public void DeleteStack()
    {
        throw new NotImplementedException();
    }

    public void EditStack()
    {
        throw new NotImplementedException();
    }

    public void ViewStacks()
    {
        IReadOnlyList<CardStack>? stacks = SafeGet(_stacksRepo.GetAll);

        if (stacks == null) return;

        var dto = MapCardStacksToDTO(stacks);
        _stacksView.DisplayStacks(dto);
    }

    private IReadOnlyList<CardStackDTO> MapCardStacksToDTO(IReadOnlyList<CardStack> stacks)
    {
        List<CardStackDTO> dto = new();
        foreach (var item in stacks)
        {
            dto.Add(new CardStackDTO
            {
                Name = item.Name,
                CardCount = 0,
            });
        }

        return dto.AsReadOnly<CardStackDTO>();
    }

    private List<CardStack>? SafeGet(Func<List<CardStack>> retrieval)
    {
        try
        {
            return retrieval();
        }
        catch (Exception ex)
        {
            _stacksView.DisplayError($"Unexpected error: {ex.Message}");
            return null;
        }
    }
}
