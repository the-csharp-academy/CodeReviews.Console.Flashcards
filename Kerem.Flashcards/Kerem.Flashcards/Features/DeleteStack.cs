using Kerem.Flashcards.Domain.Entities;
using Kerem.Flashcards.Domain.Interfaces;
using Spectre.Console;

namespace Kerem.Flashcards.Features ;

    public class DeleteStack
    {
        private readonly IStackRepository _stackRepository;
        public  DeleteStack(IStackRepository stackRepository)
        {
            _stackRepository = stackRepository;
        }

        public void Delete()
        {
            _stackRepository.GetAllStacks();
            AnsiConsole.MarkupLine("[green]Please select Stack by name to delete[/]");
            var selectedStack = Console.ReadLine() ?? " ";

            if (string.IsNullOrEmpty(selectedStack))
            {
                AnsiConsole.MarkupLine("[red]Name cannot be empty[/]");
                return;
            }
            var retrievedStack = _stackRepository.GetStackByName(selectedStack);
            AnsiConsole.MarkupLine("[green]Are you sure you want to delete this stack? This will delete its flashcards and study sessions. Type any key to abort or y to delete[/]");
            var choice  = Console.ReadLine() ?? " ";

            if (string.IsNullOrEmpty(choice))
            {
                AnsiConsole.MarkupLine("[red]You must make a choice[/]");
                return;
            }
            
            if (choice.Equals("y"))
            {
                _stackRepository.Delete(retrievedStack);
            }
           
        }
    }