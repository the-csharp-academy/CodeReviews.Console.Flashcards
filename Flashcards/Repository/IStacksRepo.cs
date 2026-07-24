public interface IStacksRepo
{
    List<CardStack> GetAll();
    void Add(string name);
    void Update(int stackId, string newName);
    void Delete(int stackId);
    CardStack? GetStackByName(string name);
}