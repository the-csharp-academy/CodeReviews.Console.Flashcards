public interface IStacksRepo
{
    List<CardStack> GetAll();
    void Add(string name);
    void Update(long stackId, string newName);
    void Delete(long stackId);
    CardStack? GetStackByName(string name);
}