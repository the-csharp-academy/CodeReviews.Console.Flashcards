public interface IStacksView
{
    void DisplayError(string message);
    void DisplayMessage(string message);
    StacksOption ShowStacksOption();
    void DisplayStacks(IReadOnlyList<CardStackDTO> stacks);
    string AskForStackName();

}