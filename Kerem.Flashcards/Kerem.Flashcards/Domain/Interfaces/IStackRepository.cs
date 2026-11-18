using Kerem.Flashcards.Domain.Entities;

namespace Kerem.Flashcards.Domain.Interfaces ;

    public interface IStackRepository
    {
        string Create(Stack stack);
        bool CheckDuplicate(Stack stack);
        Stack GetStackByName(string name);
        List<Stack> GetAllStacks();
        bool Delete(Stack stack);
    }