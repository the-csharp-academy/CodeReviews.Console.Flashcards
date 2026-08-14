namespace Flashcards;

using Microsoft.Data.SqlClient;
using Dapper;
using Spectre.Console;
using System.Data.Common;

public class DbConnector(string connectionString, string stacks, string flashcards, string studySessions)
{

    UserInput userInput = new UserInput();
    public void CreateNewStack()
    {
        string name = userInput.StringInput("Please insert the name for the new stack (MAX 100 chars):");

        using var connection = new SqlConnection(connectionString);

        connection.Execute($"INSERT INTO {stacks} (Name) VALUES (@Name)",
        new
        {
            Name = name
        }
        );

        Console.WriteLine($"Stack created successfully. Name: {name}");

        return;
    }

    public void ShowAllStacks()
    {
        List<Stacks> stacks = CreateListStacks();
        PrintListStacks(stacks);
    }

    public void DeleteStack()
    {
        ShowAllStacks();

        int stackId = CheckIdExists("Insert the stack number you want to delete, enter -1 to exit:");

        if (stackId == -1) return;

        using var connection = new SqlConnection(connectionString);

        connection.Execute(
            $"DELETE FROM {stacks} WHERE Id = @id",
            new
            {
                id = stackId
            }
        );

        Console.WriteLine("Stack deleted successfully.");

        return;
    }

    public void CreateNewFlashcard()
    {
        ShowAllStacks();

        int stackId = CheckIdExists("Select the stack number you want to create the flashcard for, enter -1 to exit:");

        if (stackId == -1) return;

        string front = userInput.StringInput("Insert the text for the front of the flashcard (MAX 100 chars):").Trim();

        string back = userInput.StringInput("Insert the text for the back of the flashcard (MAX 100 chars):").Trim();

        using var connection = new SqlConnection(connectionString);

        connection.Execute(
            $"INSERT INTO {flashcards} (Front, Back, StackId) VALUES (@Front, @Back, @StackId)",
            new
            {
                Front = front,
                Back = back,
                StackId = stackId
            }
        );
    }

    public void ShowAllFlashcardsInStack()
    {
        ShowAllStacks();

        int stackId = CheckIdExists("Select the stack number you want to see the flashcards for, enter -1 to exit:");

        if (stackId == -1) return;

        List<Flashcards> stackFlashcards = CreateListFlashcards(stackId);

        List<FlashcardsDto> flashcardsDto = stackFlashcards
            .Select(f => new FlashcardsDto { Id = f.Id, Front = f.Front, Back = f.Back })
            .ToList();

        int counter = PrintFlashcardsList(flashcardsDto);
    }

    public void DeleteFlashcard()
    {
        ShowAllStacks();

        int stackId = CheckIdExists("Select the stack number you want to see the flashcards for, enter -1 to exit:");

        if (stackId == -1) return;

        using var connection = new SqlConnection(connectionString);

        List<Flashcards> stackFlashcards = CreateListFlashcards(stackId);

        List<FlashcardsDto> flashcardsDto = stackFlashcards
            .Select(f => new FlashcardsDto { Id = f.Id, Front = f.Front, Back = f.Back })
            .ToList();

        int counter = PrintFlashcardsList(flashcardsDto);
        bool validIdList = false;
        int idList = -1;
        do
        {
            idList = Convert.ToInt32(userInput.NumberInput("Enter the number of flashcard you want to delete:"));

            if (idList < 1 || idList > counter)
            {
                Console.WriteLine("Invalid Id");
            }
            else
            {
                validIdList = true;
            }
        } while (!validIdList);

        int id = flashcardsDto[idList - 1].Id;

        connection.Execute(
            $"DELETE FROM {flashcards} WHERE Id = @Id",
            new
            {
                Id = id
            }
        );

        Console.WriteLine("Flashcard deleted successfully.");

    }

    public void StudyFlashcards()
    {
        ShowAllStacks();

        int stackId = CheckIdExists("Select the stack number you want to see the flashcards for, enter -1 to exit:");

        if (stackId == -1) return;

        List<Flashcards> stackFlashcards = CreateListFlashcards(stackId);

        if (stackFlashcards.Count == 0)
        {
            Console.WriteLine("\nThere are no flashcards in this stack yet.\n");
            return;
        }

        List<FlashcardsDto> flashcardsDto = stackFlashcards
            .Select(f => new FlashcardsDto { Id = f.Id, Front = f.Front, Back = f.Back })
            .ToList();

        Console.WriteLine("\nMatch what is in the back of the flashcard: (case insensitive)\n");

        int rightFlashcards = 0;

        foreach (var flashcard in flashcardsDto)
        {
            Console.WriteLine(flashcard.Front);
            string back = userInput.StringInput("Your answer:").Trim();

            if (back.ToUpper() == flashcard.Back!.ToUpper())
            {
                rightFlashcards++;
                Console.WriteLine("Right answer!\n");
            }
            else
            {
                Console.WriteLine("Wrong answer!\n");
            }
        }

        Console.WriteLine($"You got {rightFlashcards} right answers out of {flashcardsDto.Count}");

        Console.WriteLine(DateOnly.FromDateTime(DateTime.Now));


        using var connection = new SqlConnection(connectionString);

        connection.Execute(
                $"INSERT INTO {studySessions} (Date, ScoredPoints, TotalPoints, StackId) VALUES (@Date, @ScoredPoints, @TotalPoints, @StackId)",
                new
                {
                    Date = DateTime.Now,
                    ScoredPoints = rightFlashcards,
                    TotalPoints = flashcardsDto.Count(),
                    StackId = stackId
                }
        );

        Console.WriteLine("\nResults saved in the StudySessions table.\n");
    }

    public void ShowResults()
    {
        ShowAllStacks();

        int stackId = CheckIdExists("Select the stack number you want to see the results for, enter -1 to exit:");

        if (stackId == -1) return;

        List<StudySessions> session = CreateListSessions(stackId);

        PrintSessionsList(session);

    }

    public void ShowResultsByYear()
    {
        int year = userInput.YearInput("Please enter the year you want to filter the results for:");

        List<Stacks> stacks = CreateListStacks();

        AnsiConsole.MarkupLine("[Green]Filtered results:[/]");

        var table = CreateTable();

        foreach (var stack in stacks)
        {
            List<StudySessions> sessions = CreateListSessions(stack.Id);

            List<StudySessions> filteredSessions = sessions.Where(s => s.Date.Year == year).ToList();

            List<int> monthlySession = new List<int>();

            for (int i = 1; i < 13; i++)
            {
                List<StudySessions> filteredByMonth = filteredSessions.Where(s => s.Date.Month == i).ToList();
                monthlySession.Add(filteredByMonth.Count);
            }

            List<string> row = new List<string> { stack.Id.ToString(), stack.Name! };
            row.AddRange(monthlySession.Select(m => m.ToString()));
            table.AddRow(row.ToArray());
        }
        AnsiConsole.Write(table);

    }

    public void ShowAverageByMonth()
    {
        int year = userInput.YearInput("Please enter the year you want to filter the results for:");

        List<Stacks> stacks = CreateListStacks();

        AnsiConsole.MarkupLine("[Green]Filtered results:[/]");

        var table = CreateTable();

        foreach (var stack in stacks)
        {
            List<StudySessions> sessions = CreateListSessions(stack.Id);

            List<StudySessions> filteredSessions = sessions.Where(s => s.Date.Year == year).ToList();

            List<string> monthlyAverageSession = new List<string>();

            for (int i = 1; i < 13; i++)
            {
                List<StudySessions> filteredByMonth = filteredSessions.Where(s => s.Date.Month == i).ToList();
                
                if (filteredByMonth.Count == 0)
                {
                    monthlyAverageSession.Add("-");
                }
                else
                {
                    double average = filteredByMonth.Average(s => (double)s.ScoredPoints / s.TotalPoints * 100);
                    monthlyAverageSession.Add(average.ToString("0.0") + "%");
                }
            }

            List<string> row = new List<string> { stack.Id.ToString(), stack.Name! };
            row.AddRange(monthlyAverageSession);
            table.AddRow(row.ToArray());
        }
        AnsiConsole.Write(table);
    }

    public Table CreateTable()
    {
        var table = new Table()
            .AddColumn("[Blue]Stack ID[/]")
            .AddColumn("[Blue]Stack Name[/]")
            .AddColumn("[Blue]January[/]")
            .AddColumn("[Blue]February[/]")
            .AddColumn("[Blue]March[/]")
            .AddColumn("[Blue]April[/]")
            .AddColumn("[Blue]May[/]")
            .AddColumn("[Blue]June[/]")
            .AddColumn("[Blue]July[/]")
            .AddColumn("[Blue]August[/]")
            .AddColumn("[Blue]September[/]")
            .AddColumn("[Blue]October[/]")
            .AddColumn("[Blue]November[/]")
            .AddColumn("[Blue]December[/]");

        return table;
    }

    public List<Stacks> CreateListStacks()
    {
        using var connection = new SqlConnection(connectionString);

        List<Stacks> allStacks = connection.Query<Stacks>(
            $"SELECT * FROM {stacks} ORDER BY Id"
            ).ToList();

        return allStacks;
    }

    public List<Flashcards> CreateListFlashcards(int id)
    {
        using var connection = new SqlConnection(connectionString);

        List<Flashcards> allFlashcards = connection.Query<Flashcards>(
            $"SELECT * FROM {flashcards} WHERE StackId = @Id",
            new { Id = id }
            ).ToList();

        return allFlashcards;
    }

    public List<StudySessions> CreateListSessions(int id)
    {
        using var connection = new SqlConnection(connectionString);

        List<StudySessions> sessions = connection.Query<StudySessions>(
            $"SELECT * FROM {studySessions} WHERE StackId = @Id",
            new
            { Id = id }
        ).ToList();

        foreach (var session in sessions)
        {
            Console.WriteLine(session.Id);
        }

        return sessions;
    }

    public void PrintListStacks(List<Stacks> stacks)
    {
        if (stacks.Count == 0)
        {
            Console.WriteLine("There are no stacks.");
        }
        else
        {
            AnsiConsole.MarkupLine("[Green]Stacks:[/]");

            var table = new Table()
                .AddColumn("[Blue]Id[/]")
                .AddColumn("[Blue]Name[/]");

            foreach (var stack in stacks)
            {
                table.AddRow(stack.Id.ToString(), stack.Name!);
            }
            AnsiConsole.Write(table);
        }
    }

    public int PrintFlashcardsList(List<FlashcardsDto> flashcards)
    {
        int counter = 1;
        if (flashcards.Count == 0)
        {
            Console.WriteLine("There are no fashcards in this stack.\n");
        }
        else
        {

            AnsiConsole.MarkupLine("[Green]Flashcards:[/]");
            var table = new Table()
                .AddColumn("[Blue]Id[/]")
                .AddColumn("[Blue]Front[/]")
                .AddColumn("[Blue]Back[/]");

            foreach (var flashcard in flashcards)
            {
                table.AddRow(counter.ToString(), flashcard.Front!, flashcard.Back!);
                counter++;
            }
            AnsiConsole.Write(table);
        }
        return flashcards.Count;
    }

    public void PrintSessionsList(List<StudySessions> sessions)
    {
        if (sessions.Count == 0)
        {
            Console.WriteLine("There are no studying sessions in this stack.\n");
        }
        else
        {
            List<StudySessionsDto> sessionsDto = sessions.Select(
                f => new StudySessionsDto { Id = f.Id, Date = f.Date, ScoredPoints = f.ScoredPoints, TotalPoints = f.TotalPoints }
            ).ToList();

            AnsiConsole.MarkupLine("[Green]Results:[/]");
            var table = new Table()
                .AddColumn("[Blue]Id[/]")
                .AddColumn("[Blue]Date[/]")
                .AddColumn("[Blue]Points Done[/]")
                .AddColumn("[Blue]Total Points[/]");

            foreach (var session in sessionsDto)
            {
                table.AddRow(session.Id.ToString(), session.Date.ToString("dd-MM-yyyy"), session.ScoredPoints.ToString(), session.TotalPoints.ToString());
            }
            AnsiConsole.Write(table);
        }
    }

    public int CheckIdExists(string message)
    {
        bool validNumber = false;
        string input = "";
        int number = -1;

        using var connection = new SqlConnection(connectionString);

        do
        {
            input = userInput.NumberInput(message).Trim();

            if (input == "-1") return -1;

            int exists = connection.ExecuteScalar<int>(
                $"SELECT COUNT(1) FROM {stacks} WHERE Id = @id",
                new
                {
                    id = input
                }
            );

            if (exists == 0)
            {
                Console.WriteLine("Stack given does not exist, try again.");
            }
            else
            {
                validNumber = int.TryParse(input, out number);

            }

        } while (!validNumber);


        return number;
    }
}