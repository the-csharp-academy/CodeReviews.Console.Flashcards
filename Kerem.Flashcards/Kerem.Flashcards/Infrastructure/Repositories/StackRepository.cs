using Dapper;
using Kerem.Flashcards.Domain.Entities;
using Kerem.Flashcards.Domain.Interfaces;
using Kerem.Flashcards.Infrastructure.Persistence;
using Spectre.Console;

namespace Kerem.Flashcards.Infrastructure.Repositories ;

    public class StackRepository: IStackRepository
    {

        private readonly DapperDbContext _dapperDbContext;

        public StackRepository(DapperDbContext dapperDbContext)
        {
            _dapperDbContext = dapperDbContext;
        }

        public string Create(Stack stack)
        {
            var connection = _dapperDbContext.GetConnection();
            var stackSql = "INSERT INTO Stacks(Name) VALUES (@Name)";
            connection.Execute(stackSql, stack);
            var stackIdSql = "SELECT * FROM Stacks WHERE Name = (@Name)";
            var result = connection.ExecuteScalar<int>(stackIdSql, new { Name = stack.Name });
            foreach (var flashcard in stack.Flashcards)
            {
                flashcard.StackId = result;
                var flashcardSql = "INSERT INTO Flashcards(Name, StackId) values (@Name, @StackId)";
                connection.Execute(flashcardSql,flashcard);
            }
            return "Stack created!";
        }

        public bool CheckDuplicate(Stack stack)
        {
            var connection = _dapperDbContext.GetConnection();
            var stackName = "SELECT * FROM Stacks WHERE Name = (@Name)";
            bool result = connection.ExecuteScalar<bool>(stackName, new { Name = stack.Name });
            if (!result)
            {
                return false;
            }
            return true;
        }

        public Stack GetStackByName(string name)
        {
            var connection = _dapperDbContext.GetConnection();
            var stackSql = "SELECT * FROM Stacks WHERE Name = @Name";
            var stack = connection.Query<Stack>(stackSql, new { Name = name }).FirstOrDefault();
            int stackId = stack.Id;
            var flashcard = "SELECT * FROM Flashcards WHERE StackId = @StackId";
            var result = connection.QueryMultiple(flashcard,  new { StackId = stackId });
            var stacks  = result.Read<Flashcard>().ToList();
            stack.Flashcards = stacks;
            return stack;
        }
        
        public List<Stack> GetAllStacks()
        {
            var connection = _dapperDbContext.GetConnection();
            var stackSql = "SELECT * FROM Stacks";
            List<Stack> stacks = connection.Query<Stack>(stackSql).ToList();
            return stacks;
        }
    }