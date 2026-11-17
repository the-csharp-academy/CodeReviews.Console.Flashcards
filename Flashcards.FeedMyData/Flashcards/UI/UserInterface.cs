using Microsoft.Data.SqlClient;
using Spectre.Console;

namespace Flashcards;

internal class UserInterface
{
    private Database _database;
    private int displayCount;

    internal UserInterface(Database database)
    {
        _database = database;
    }

    internal void MainMenuSelector()
    {
        while (true)
        {
            // Ensure loading stay displayed on first launch
            displayCount++;
            if (displayCount != 1)
                Displayer.CleanConsole();
            //----
            Enums.MainMenuChoice[] choices;
            try
            {
                if (_database.QueryOneStack() is null)
                    choices = [Enums.MainMenuChoice.manageStacks];

                else if (_database.QueryOneFlashcard() is null)
                    choices = [Enums.MainMenuChoice.manageStacks, Enums.MainMenuChoice.manageFlashcards];

                else if (_database.QueryOneStudy() is null)
                    choices = [Enums.MainMenuChoice.study, Enums.MainMenuChoice.manageStacks, Enums.MainMenuChoice.manageFlashcards];

                else
                    choices = Enum.GetValues<Enums.MainMenuChoice>();

                var selection = AnsiConsole.Prompt(
                    new SelectionPrompt<Enums.MainMenuChoice>()
                    .Title("What do you want to do?")
                    .HighlightStyle(Color.SeaGreen1)
                    .UseConverter(choice => choice.GetDisplayName())
                    .AddChoices(choices));

                switch (selection)
                {
                    case Enums.MainMenuChoice.study:
                        NewStudy();
                        break;
                    case Enums.MainMenuChoice.studyView:
                        ViewStudies();
                        break;
                    case Enums.MainMenuChoice.manageFlashcards:
                        CrudMenuSelector(Enums.ObjectType.flashcard);
                        break;
                    case Enums.MainMenuChoice.manageStacks:
                        CrudMenuSelector(Enums.ObjectType.stack);
                        break;
                    case Enums.MainMenuChoice.exit:
                        return;
                }
            }
            catch (SqlException ex)
            {
                AnsiConsole.MarkupLine($"{ex.GetType()}: [red]{Markup.Escape(ex.Message)}[/]");
                AnsiConsole.MarkupLine("[grey]Press any key to return to the main menu.[/]");
                Console.ReadKey();
            }
        }
    }

    private void CrudMenuSelector(Enums.ObjectType type)
    {
        while (true)
        {
            Displayer.CleanConsole();

            bool isFlashcard = type == Enums.ObjectType.flashcard;
            bool hasItems = isFlashcard
                ? _database.QueryOneFlashcard() is not null
                : _database.QueryOneStack() is not null;

            var choices = hasItems
                ? Enum.GetValues<Enums.CrudChoice>()
                : new[] { Enums.CrudChoice.addItem, Enums.CrudChoice.mainMenu };

            var selection = ChoicesPrompt.PromptCrudMenu(
                isFlashcard ? "FLASHCARDS" : "STACKS",
                choices);

            switch (selection)
            {
                case Enums.CrudChoice.mainMenu:
                    return;
                case Enums.CrudChoice.viewItem:
                    if (isFlashcard) viewFlashcard(); else viewStack();
                    break;
                case Enums.CrudChoice.addItem:
                    if (isFlashcard) AddFlashcard(); else AddStack();
                    break;
                case Enums.CrudChoice.editItem:
                    if (isFlashcard) EditFlashcard(); else EditStack();
                    break;
                case Enums.CrudChoice.deleteItem:
                    if (isFlashcard) DeleteFlashcard(); else DeleteStack();
                    break;
            }
        }
    }

    //------------------------------------------
    //----------------- VIEW -------------------
    //------------------------------------------

    void viewFlashcard()
    {
        while (true)
        {
            AnsiConsole.MarkupLine("[yellow]View [bold]FLASHCARDS[/][/]");
            AnsiConsole.WriteLine();

            List<Flashcard> flashcards = FilterSelectIfNeeded("yellow");

            Displayer.PrintTable(ModelsMapper.FlashcardToDto(flashcards));
            AnsiConsole.WriteLine();

            if (!AnsiConsole.Confirm("View another table?"))
                break;

            AnsiConsole.WriteLine();
        }
    }

    void viewStack()
    {
        while (true)
        {
            AnsiConsole.MarkupLine("[yellow]View [bold]STACKS[/][/]");
            AnsiConsole.WriteLine();

            List<Stack> stacks = _database.QueryAllFromStacks();
            stacks.Sort((x, y) => x.Name.CompareTo(y.Name));

            foreach (Stack stack in stacks)
            {
                AnsiConsole.MarkupLine(stack.Name);
            }
            AnsiConsole.WriteLine();
            Displayer.GoBackPressKey();
        }
    }

    void ViewStudies()
    {
        AnsiConsole.MarkupLine("[MediumPurple]Previous [bold]STUDIES[/][/]");
        AnsiConsole.WriteLine();

        List<StudyDto> studies = _database.QueryMergeStudiesAndStacksNoId();

        Displayer.PrintTable(studies);
        Displayer.GoBackPressKey();
    }

    //------------------------------------------
    //----------------- ADD -------------------
    //------------------------------------------

    void AddFlashcard()
    {
        while (true)
        {
            AnsiConsole.MarkupLine("[green]New [bold]FLASHCARD[/][/]");
            AnsiConsole.WriteLine();

            // Display an "empy" stack if the user needs a new one
            Stack newStack = new(-1, "_New Stack");
            List<Stack> stacks = _database.QueryAllFromStacks();
            stacks.Add(newStack);
            //stacks.Sort((x, y) => x.Name.CompareTo(y.Name));

            var stack = ChoicesPrompt.Select(stacks, "Choose a stack", "green");
            if (stack.StackId == -1)
            {
                if (!AddStackOnce())
                {
                    Displayer.GenericError("Operation aborpted: a flashcard need a stack.");
                    break;
                }

                stack = _database.QueryNewestStack();
            }

            AnsiConsole.MarkupLine($"Choosen stack: {stack.Name}");

            var recto = AnsiConsole.Ask<string>("Add a recto :");
            var verso = AnsiConsole.Ask<string>("Add a verso :");

            AnsiConsole.WriteLine();
            bool confirm = AnsiConsole.Confirm("Confirm and add the flashcard?");

            if (confirm)
            {
                _database.AddFlashcard(stack.StackId, recto, verso);
                Displayer.CrudSuccess("flashcard", "added");
            }
            else
                Displayer.CrudDiscard("flashcard", "discarded");

            if (!AnsiConsole.Confirm("Add another flashcard?"))
                break;

            AnsiConsole.WriteLine();
        }
    }

    void AddStack()
    {
        while (true)
        {
            AddStackOnce();

            if (!AnsiConsole.Confirm("Add another Stack?"))
                break;

            AnsiConsole.WriteLine();
        }
    }

    bool AddStackOnce()
    {
        AnsiConsole.MarkupLine("[green]New [bold]STACK[/][/]");
        AnsiConsole.WriteLine();

        var stacks = _database.QueryAllFromStacks();
        var name = AnsiConsole.Ask<string>("Add a name :");

        var existingNames = stacks.Select(s => s.Name).ToHashSet();

        while (existingNames.Contains(name))
        {
            AnsiConsole.MarkupLine($"[Orange1]The name is already used. Choose another name.[/]");
            name = AnsiConsole.Ask<string>("Add a name :");
        }

        bool confirm = AnsiConsole.Confirm("Add the stack?");

        if (confirm)
        {
            _database.AddStack(name);
            Displayer.CrudSuccess("stack", "added");
        }
        else
            Displayer.CrudDiscard("stack", "discarded");

        return confirm;
    }

    void NewStudy()
    {
        int score = 0;
        int index = 0;
        DateOnly date = DateOnly.FromDateTime(DateTime.Now);
        System.Diagnostics.Stopwatch stopwatch = new();

        Stack stack = ChooseStack("MediumPurple");
        List<Flashcard> flashcards = _database.QueryFlashcardsFromStack(stack);
        if (flashcards.Count == 0)
        {
            Displayer.GenericError("The selected stack is empty.");
            return;
        }

        foreach (Flashcard f in flashcards)
        {
            index++;

            Displayer.CleanConsole();
            AnsiConsole.MarkupLine("[MediumPurple]New [bold]Study[/][/]");
            AnsiConsole.WriteLine();
            AnsiConsole.MarkupLine($"[MediumPurple]Question number:[/] {index}");
            AnsiConsole.MarkupLine($"[bold]Score:[/] {score}/{flashcards.Count}");
            AnsiConsole.WriteLine();

            Displayer.PrintPanel(f.Recto, Color.MediumPurple, stack.Name);
            Console.WriteLine();
            string answer = AnsiConsole.Ask<string>("Your answer: ");

            if (AnswerIsCorrect(answer, f.Verso))
            {
                score++;
                AnsiConsole.MarkupLine($"[green]{Displayer.GoodAnswer()}[/]");
            }

            else
                AnsiConsole.MarkupLine($"[red]{Displayer.WrongAnswer()} It was:[/] [bold]{f.Verso}[/]");


            AnsiConsole.MarkupLine("[grey]Press any key to continue or Esc to exit.[/]");
            if (Console.ReadKey(true).Key == ConsoleKey.Escape)
                return;
        }

        Displayer.CleanConsole();
        AnsiConsole.MarkupLine($"[MediumPurple]------------------------------ [bold]Final score:[/] {score}/{flashcards.Count} -----------------------------[/]");

        if (score == flashcards.Count && score >= 1)
        {
            Displayer.MaxScoreCongrats();
        }

        _database.AddStudy(stack.StackId, date, score, flashcards.Count);


        Displayer.GoBackPressKey();
    }

    bool AnswerIsCorrect(string a, string b)
    {
        return a.ToLower().Trim() == b.ToLower().Trim();
    }

    //------------------------------------------
    //----------------- EDIT -------------------
    //------------------------------------------

    void EditFlashcard()
    {
        while (true)
        {
            AnsiConsole.MarkupLine("[blue]Edit [bold]FLASHCARD[/][/]");
            AnsiConsole.WriteLine();
            List<Flashcard> flashcards = FilterSelectIfNeeded("blue");

            if (flashcards.Count == 0)
            {
                Displayer.GenericError("The selected stack is empty.");
                break;
            }

            AnsiConsole.WriteLine();

            var selected = ChoicesPrompt.Select(flashcards, "Choose a Flashcard", "blue");
            string newRecto = selected.Recto;
            string newVerso = selected.Verso;

            string[] subChoice = { "recto", "verso" };
            List<string> dataToEdit = ChoicesPrompt.SelectMulti(subChoice, "What do you want to Edit?", "blue");
            AnsiConsole.MarkupLine($"[blue]Currently editing:[/] '{selected.Display}'");


            if (dataToEdit.Count == 0)
            {
                Displayer.GenericError("No data to edit.");
                break;
            }

            if (dataToEdit.Contains("recto"))
                selected.EditRecto(AnsiConsole.Ask<string>("Enter a new recto: "));
            if (dataToEdit.Contains("verso"))
                selected.EditVerso(AnsiConsole.Ask<string>("Enter a new verso: "));

            if (AnsiConsole.Confirm("Confirm?"))
            {
                _database.UpdateFlashcard(selected);
                Displayer.CrudSuccess("edit", "successful");
            }
            else
                Displayer.CrudDiscard("edit", "canceled");

            if (!AnsiConsole.Confirm("Edit other flashcards?"))
                break;

            AnsiConsole.WriteLine();
        }
    }

    void EditStack()
    {
        while (true)
        {
            AnsiConsole.MarkupLine("[blue]Edit [bold]STACK[/][/]");
            AnsiConsole.WriteLine();
            List<Stack> stacks = _database.QueryAllFromStacks();

            var selected = ChoicesPrompt.Select(stacks, "Choose a stack", "blue");

            AnsiConsole.MarkupLine($"[blue]Currently editing:[/] '{selected.Name}'");

            selected.EditName(AnsiConsole.Ask<string>("Enter a new name: "));

            if (AnsiConsole.Confirm("Confirm?"))
            {
                _database.UpdateStack(selected);
                Displayer.CrudSuccess("edit", "successful");
            }

            else
                Displayer.CrudDiscard("edit", "canceled");

            if (!AnsiConsole.Confirm("Edit other stacks?"))
                break;

            AnsiConsole.WriteLine();
        }
    }

    //------------------------------------------
    //----------------- DELETE -----------------
    //------------------------------------------

    void DeleteFlashcard()
    {
        while (true)
        {
            AnsiConsole.MarkupLine($"[red]Delete [bold]FLASHCARDS[/][/]");
            AnsiConsole.WriteLine();
            List<Flashcard> flashcards = FilterSelectIfNeeded("red");
            if (flashcards.Count == 0)
            {
                Displayer.GenericError("There is no flaschards to delete in the selection.");
                break;
            }

            var toDelete = ChoicesPrompt.SelectMulti(flashcards, "red");
            if (toDelete.Count == 0)
                return;

            foreach (Flashcard f in toDelete)
                AnsiConsole.MarkupLine($"Recto -> {f.Recto}");

            AnsiConsole.WriteLine();

            if (AnsiConsole.Confirm($"[red]Delete {toDelete.Count} Flashcards?[/]"))
            {
                foreach (Flashcard f in toDelete)
                    _database.DeleteFlashcard(f);

                Displayer.CrudSuccess("deletion", "successful");
            }
            else
                Displayer.CrudDiscard("deletion", "canceled");

            if (_database.QueryOneFlashcard() == null)
            {
                Displayer.GoBackPressKey();
                break;
            }

            if (!AnsiConsole.Confirm("Delete other Flashcard?"))
                break;

            AnsiConsole.WriteLine();
        }
    }

    void DeleteStack()
    {
        while (true)
        {
            AnsiConsole.MarkupLine("[red]Delete [bold]STACKS[/][/]");
            AnsiConsole.WriteLine();

            List<Stack> stacks = _database.QueryAllFromStacks();
            var toDelete = ChoicesPrompt.SelectMulti(stacks, "red");
            if (toDelete.Count == 0)
                break;

            foreach (Stack stack in toDelete)
                AnsiConsole.MarkupLine($"[red]Delete -> {stack.Name}[/]");

            AnsiConsole.WriteLine();

            if (AnsiConsole.Confirm("Confirm?"))
            {
                foreach (Stack stack in toDelete)
                    _database.DeleteStack(stack);

                Displayer.CrudSuccess("deletion", "successful");
            }
            else
                Displayer.CrudDiscard("deletion", "canceled");

            if (_database.QueryOneStack() == null)
            {
                Displayer.GoBackPressKey();
                break;
            }

            if (!AnsiConsole.Confirm("Delete other Stacks?"))
                break;

            AnsiConsole.WriteLine();
        }
    }

    //------------------------------------------
    //----------------- HELPERS ----------------
    //------------------------------------------

    private List<Flashcard> FilterSelectIfNeeded(string color)
    {
        var filter = AnsiConsole.Confirm("Filter per stack?");

        if (filter)
        {
            Stack stack = ChooseStack(color);
            AnsiConsole.MarkupLine($"[bold]{stack.Name}[/]");

            return _database.QueryFlashcardsFromStack(stack);
        }


        else
            return _database.QueryAllFromFlashcards();
    }

    private Stack ChooseStack(string color)
    {
        List<Stack> stacks = _database.QueryAllFromStacks();
        return ChoicesPrompt.Select(stacks, "Choose a stack", color);
    }
}
