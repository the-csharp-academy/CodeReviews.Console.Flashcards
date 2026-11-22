using Kerem.Flashcards.Domain.Entities;
using Kerem.Flashcards.Domain.Interfaces;
using Spectre.Console;

namespace Kerem.Flashcards.Features ;

    public class CreateStudySession
    {
        IStudySessionRepository _repository;
        IStackRepository _stackRepository;

        public CreateStudySession(IStudySessionRepository repository,  IStackRepository stackRepository)
        {
            _repository = repository;
            _stackRepository = stackRepository;
        }

        public void Study()
        {
            AnsiConsole.MarkupLine("[red]Welcome to the study session. Here you will have to fill out the missing letter in the word[/]");
            AnsiConsole.MarkupLine("[red]Please select a study stack.[/]");

            List<Stack> stacks = _stackRepository.GetAllStacks();
            var table = new Table();
            
            table.AddColumn("Name");
            foreach (var stack in stacks)
            {
                var stackDtoObject = StackMapper.MapToDto(stack);
                table.AddRow(stackDtoObject.Name);
            }
            AnsiConsole.Write(table);

            var stackChoice = Console.ReadLine() ?? " ";
            StudySession studySession = new StudySession();
            
            var stackSelected = _stackRepository.GetStackByName(stackChoice);

            if (stackSelected == null)
            {
                AnsiConsole.MarkupLine($"[red]Stack not found.[/]");
                return;
            }
            
            studySession.StackId = stackSelected.Id;
            studySession.Stack = stackSelected;

            int score = 0;
            foreach (var flashcard in stackSelected.Flashcards)
            {
                Console.WriteLine(flashcard.Name.Remove(2) + " " + flashcard.Name.Substring(3));
                Console.WriteLine("Please complete the word");
                var completedWord = Console.ReadLine() ?? " ";
                if (completedWord.Equals(flashcard.Name))
                {
                    score++;
                    AnsiConsole.MarkupLine($"[green]Correct![/]");
                }
                if (!completedWord.Equals(flashcard.Name))
                {
                    AnsiConsole.MarkupLine($"[red]Incorrect[/]");
                }
            }
            studySession.CreatedAt = DateTime.Now;
            studySession.Score = score;
            string message = _repository.Create(studySession);
            AnsiConsole.MarkupLine($"[green]{message}[/]");
        }
    }