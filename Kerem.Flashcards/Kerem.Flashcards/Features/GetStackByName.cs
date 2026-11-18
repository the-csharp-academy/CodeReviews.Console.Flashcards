using Kerem.Flashcards.Domain.Entities;
using Kerem.Flashcards.Domain.Interfaces;
using Spectre.Console;

namespace Kerem.Flashcards.Features ;

    public class GetStackByName
    {
        private readonly IStackRepository _repository;
        
        public  GetStackByName(IStackRepository repository)
        {
            this._repository = repository;
        }
        
        public void SelectStack()
        {
            List<Stack> stacks = _repository.GetAllStacks();
            var table = new Table();
            
            table.AddColumn("Name");
            foreach (var stack in stacks)
            {
                var stackDtoObject = StackMapper.MapToDto(stack);
                table.AddRow(stackDtoObject.Name);
            }
            AnsiConsole.Write(table);
            
            AnsiConsole.MarkupLine("Please select a stack from the list by name");
            var input = Console.ReadLine() ?? " ";

            if (string.IsNullOrEmpty(input))
            {
                AnsiConsole.MarkupLine("[red]Name cannot be empty[/]");
                return;
            }
            var selectedStack = _repository.GetStackByName(input);
            if (selectedStack == null)
            {
                AnsiConsole.MarkupLine("[red]Stack does not exist[/]");
                return;
            }
            Console.WriteLine(selectedStack.Name);
            foreach (var flashcard in selectedStack.Flashcards)
            {
                Console.WriteLine(flashcard.Name);
            }
        }
    }