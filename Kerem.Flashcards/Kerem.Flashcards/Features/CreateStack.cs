using Kerem.Flashcards.Domain.Entities;
using Kerem.Flashcards.Domain.Interfaces;
using Kerem.Flashcards.Infrastructure.Utils;
using Spectre.Console;

namespace Kerem.Flashcards.Features ;

    public class CreateStack
    {
        private readonly IStackRepository _stackRepository;

        public CreateStack(IStackRepository stackRepository)
        {
            _stackRepository = stackRepository;
        }

        public void Create()
        {
            Stack stack = new Stack();
            AnsiConsole.MarkupLine("[bold steelblue]Please enter the name of the stack[/]");
            AnsiConsole.MarkupLine("[Orange1]Enter abort to exit back to the main menu[/]");
            
            string stackName = Console.ReadLine() ?? " ";
            bool shouldAbort = Validator.Abort(stackName);
            
            if (shouldAbort)
            {
                AnsiConsole.MarkupLine("[Orange1]Aborted[/]");
                return;
            }

            if (string.IsNullOrEmpty(stackName))
            {
                AnsiConsole.MarkupLine("[red]Name cannot be empty[/]");
                return;
            }
            stack.Name = stackName;
            bool isDuplicate = _stackRepository.CheckDuplicate(stack);

            if (isDuplicate)
            {
                AnsiConsole.MarkupLine("[red]Stack already exists[/]");
                return;
            }

            AnsiConsole.MarkupLine("[bold steelblue]How many flashcards to you want to crate?[/]");

            int amount = Convert.ToInt32(Console.ReadLine() ?? "0");

            if (amount <= 0)
            {
                AnsiConsole.MarkupLine("[red]Format is wrong, please enter a number or a number greater than 0[/]");
                return;
            }
            
            for (int i = 0; i < amount; i++)
            {
                Flashcard flashcard = new Flashcard();
                AnsiConsole.MarkupLine("[bold steelblue]Please enter the name of the flashcard[/]");

                var flashcardName = Console.ReadLine() ?? " ";
                
                if (string.IsNullOrEmpty(flashcardName))
                {
                    AnsiConsole.MarkupLine("[red]Name cannot be empty[/]");
                    return;
                }

                flashcard.Name = flashcardName;
                stack.Flashcards.Add(flashcard);
            }
            
            string message = _stackRepository.Create(stack);
            AnsiConsole.MarkupLine($"[green]{message}[/]");
            
        }
    }
    
    