using Spectre.Console;

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
        string stackName = _stacksView.AskForStackName();
        if (!RepositoryHelpers.TryExecute(
            () => _stacksRepo.Add(stackName),
            out Exception? exception))
        {
            _stacksView.DisplayError($"Could not add stack: {exception!.Message}");
            return;
        }

        _stacksView.DisplayMessage("Stack added successfully.");
    }

    public void DeleteStack()
    {
        string stackName = _stacksView.AskForStackName();
        if (!TryGetStackByName(stackName, out CardStack stackToDelete))
            return;

        if (!RepositoryHelpers.TryExecute(
                () => _stacksRepo.Delete(stackToDelete.StackId),
                out Exception? exception))
        {
            _stacksView.DisplayError($"Could not delete stack: {exception!.Message}");
            return;
        }

        _stacksView.DisplayMessage("Stack deleted successfully.");
    }

    public void EditStack()
    {
        string currentName = _stacksView.AskForStackName();
        if (!TryGetStackByName(currentName, out CardStack stackToEdit))
            return;

        string newName = _stacksView.AskForStackName();

        if (!RepositoryHelpers.TryExecute(
                () => _stacksRepo.Update(stackToEdit.StackId, newName),
                out Exception? exception))
        {
            _stacksView.DisplayError($"Could not update stack: {exception!.Message}");
            return;
        }

        _stacksView.DisplayMessage("Stack updated successfully.");
    }

    public void ViewStacks()
    {
        if (!TryGetAllStacks(out IReadOnlyList<CardStack> stacks))
            return;
        var dto = MapCardStacksToDTO(stacks);
        _stacksView.DisplayStacks(dto);
        _stacksView.WaitForInput();
    }

    private static IReadOnlyList<CardStackDTO> MapCardStacksToDTO(IReadOnlyList<CardStack> stacks)
    {
        List<CardStackDTO> dto = new();
        foreach (var item in stacks)
        {
            dto.Add(new CardStackDTO { Name = item.Name });
        }

        return dto.AsReadOnly();
    }
    private bool TryGetAllStacks(out IReadOnlyList<CardStack> stacks)
    {
        stacks = Array.Empty<CardStack>();
        try
        {
            stacks = _stacksRepo.GetAll();
            return true;
        }
        catch (Exception ex)
        {
            _stacksView.DisplayError($"Could not retrieve stacks: {ex.Message}");
            return false;
        }
    }

    private bool TryGetStackByName(string stackName, out CardStack stack)
    {
        stack = null!;
        try
        {
            CardStack? foundStack = _stacksRepo.GetStackByName(stackName);
            if (foundStack == null)
            {
                _stacksView.DisplayError($"Stack '{stackName}' could not be found.");
                return false;
            }

            stack = foundStack;
            return true;
        }
        catch (Exception ex)
        {
            _stacksView.DisplayError($"Could not retrieve stack: {ex.Message}");
            return false;
        }
    }
}
