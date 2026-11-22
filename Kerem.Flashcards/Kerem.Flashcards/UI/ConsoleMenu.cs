using Kerem.Flashcards.Domain.Entities;
using Kerem.Flashcards.Features;
using Spectre.Console;

namespace Kerem.Flashcards.UI;

    public class ConsoleMenu
    {
        private readonly CreateStack _createStack;
        private readonly GetStackByName _getStackByName;
        private readonly DeleteStack _deleteStack;
        private readonly CreateStudySession _studySession;
        private readonly GetAllStudySessions _getAllStudySessions;
        private readonly GetAllStacks _getAllStacks;

        public ConsoleMenu(CreateStack createStack,  GetStackByName getStackByName , DeleteStack deleteStack, CreateStudySession studySession,  GetAllStudySessions getAllStudySessions,  GetAllStacks getAllStacks)
        {
            _createStack = createStack;
            _getStackByName = getStackByName;
            _deleteStack = deleteStack;
            _studySession = studySession;
            _getAllStudySessions = getAllStudySessions;
            _getAllStacks = getAllStacks;
        }

        public void Menu()
        {
            AnsiConsole.Write(new FigletText("LearnALanguage"));
            while (true)
            {
                string choice = AnsiConsole.Prompt(
                    new SelectionPrompt<string>()
                        .Title("Please select an [bold steelblue]option[/]:")
                        .AddChoices(
                            "1. Create a stack",
                            "2. Display a stack",
                            "3. Display all stacks",
                            "4  Delete a stack",
                            "5. Start a study session",
                            "6. Display all study sessions",
                            "7. Exit"));

                switch (choice[0])
                {
                    case '1':
                        _createStack.Create();
                        break;
                    case '2':
                        _getAllStacks.GetAll();
                        _getStackByName.SelectStack();
                        break;
                    case '3':
                        _getAllStacks.GetAll();
                        break;
                    case '4':
                        _getAllStacks.GetAll();
                        _deleteStack.Delete();
                        break;
                    case '5':
                        _studySession.Study();
                        break;
                    case '6':
                        _getAllStudySessions.GetAll();
                        break;
                    case '7':
                        AnsiConsole.MarkupLine("[bold green]Goodbye![/]");
                        return;
                }
            }
        }
    }
    
