using Kerem.Flashcards.Domain.Interfaces;
using Spectre.Console;

namespace Kerem.Flashcards.Features ;

    public class GetAllStacks
    {
        IStackRepository _repository;

        public GetAllStacks(IStackRepository repository)
        {
            _repository = repository;
        }

        public void GetAll()
        {
            var stacks = _repository.GetAllStacks();
            if (!stacks.Any())
            {
                AnsiConsole.MarkupLine("No stacks found");
                return;
            }
            
            var table = new Table();
            
            table.AddColumn("Name");
            foreach (var stack in stacks)
            {
                var stackDtoObject = StackMapper.MapToDto(stack);
                table.AddRow(stackDtoObject.Name);
            }
            AnsiConsole.Write(table);
        }
    }