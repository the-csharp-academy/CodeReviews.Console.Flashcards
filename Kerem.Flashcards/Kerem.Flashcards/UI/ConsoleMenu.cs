using Kerem.Flashcards.Features;
using Spectre.Console;

namespace Kerem.Flashcards.UI;

    public class ConsoleMenu
    {
        private readonly CreateStack _createStack;
        private readonly GetStackByName _getStackByName;

        public ConsoleMenu(CreateStack createStack,  GetStackByName getStackByName)
        {
            _createStack = createStack;
            _getStackByName = getStackByName;
        }

        public void Menu()
        {
            AnsiConsole.Write(new FigletText("Flashcard App"));
            while (true)
            {
                string choice = AnsiConsole.Prompt(
                    new SelectionPrompt<string>()
                        .Title("Please select an [bold steelblue]option[/]:")
                        .AddChoices(
                            "1. Create a stack",
                            "2. Display a stack",
                            "3. Exit"));

                switch (choice[0])
                {
                    case '1':
                        _createStack.Create();
                        break;
                    case '2':
                        _getStackByName.SelectStack();
                        break;
                    case '3':
                        AnsiConsole.MarkupLine("[bold green]Goodbye![/]");
                        return;
                }
            }
        }
    }
    
