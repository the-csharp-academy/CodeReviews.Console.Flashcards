using Dapper;
using Flashcards;
using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Configuration;

var config = new ConfigurationBuilder()
    .SetBasePath(Directory.GetCurrentDirectory())
    .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
    .Build();

string connectionString = config.GetConnectionString("DefaultConnection")!;
string masterConnectionString = config.GetConnectionString("MasterConnection")!;
string stacks = config.GetSection("TableNames")["Stacks"]!;
string flashcards = config.GetSection("TableNames")["Flashcards"]!;
string studySessions = config.GetSection("TableNames")["StudySessions"]!;

void CreateDbAndTablesIfNotExists()
{
    using var masterConnection = new SqlConnection(masterConnectionString);
    masterConnection.Execute(
        @"IF NOT EXISTS (SELECT * FROM sys.databases WHERE name = 'FlashcardsDb')
        BEGIN
            CREATE DATABASE FlashcardsDb
        END"
    );

    using var connection = new SqlConnection(connectionString);
    connection.Execute(
        $@"IF NOT EXISTS (SELECT * FROM sys.tables WHERE name = '{stacks}')
        BEGIN
            CREATE TABLE {stacks} (
                Id INT PRIMARY KEY IDENTITY(1,1) NOT NULL,
                Name NVARCHAR(100) NOT NULL UNIQUE
            )  
        END"
    );

    connection.Execute(
        $@"IF NOT EXISTS (SELECT * FROM sys.tables WHERE name = '{flashcards}')
        BEGIN
            CREATE TABLE {flashcards} (
                Id INT PRIMARY KEY IDENTITY(1,1) NOT NULL,
                Front NVARCHAR(100) NOT NULL,
                Back NVARCHAR(100) NOT NULL,
                StackId INT NOT NULL,
                FOREIGN KEY (StackId) REFERENCES {stacks}(Id) ON DELETE CASCADE
            )  
        END"
    );

    connection.Execute(
        $@"IF NOT EXISTS (SELECT * FROM sys.tables WHERE name = '{studySessions}')
        BEGIN
            CREATE TABLE {studySessions} (
                Id INT PRIMARY KEY IDENTITY(1,1) NOT NULL,
                Date DATETIME NOT NULL,
                ScoredPoints INT NOT NULL,
                TotalPoints INT NOT NULL,
                StackId INT NOT NULL,
                FOREIGN KEY (StackId) REFERENCES {stacks}(Id) ON DELETE CASCADE
            )  
        END"
    );
}

CreateDbAndTablesIfNotExists();

UserInput userInput = new UserInput();
DbConnector dbConnector = new DbConnector(connectionString, stacks, flashcards, studySessions);

bool wantToExit = false;

while (!wantToExit)
{
    Console.WriteLine("\tMENU\n");
    Console.WriteLine("\tMANAGE STACKS");
    Console.WriteLine("1. Create a new stack");
    Console.WriteLine("2. view all stacks");
    Console.WriteLine("3. delete a stacks\n");

    Console.WriteLine("\tMANAGE FLASHCARDS");
    Console.WriteLine("4. Create a new flashcard");
    Console.WriteLine("5. View all flashcards of a stack");
    Console.WriteLine("6. Delete a flashcard of a stack\n");

    Console.WriteLine("\tSTUDY");
    Console.WriteLine("7. Study a stack\n");

    Console.WriteLine("\tSTUDY SESSIONS DATA");
    Console.WriteLine("8. Check study sessions results");
    Console.WriteLine("9. Check study sessions results by year");
    Console.WriteLine("10. Check average study sessions results by month\n");

    Console.WriteLine("11. Exit");

    string input = userInput.NumberInput("");
    try
    {
        switch (input)
        {
            case "1":
                dbConnector.CreateNewStack();
                break;
            case "2":
                dbConnector.ShowAllStacks();
                break;
            case "3":
                dbConnector.DeleteStack();
                break;
            case "4":
                dbConnector.CreateNewFlashcard();
                break;
            case "5":
                dbConnector.ShowAllFlashcardsInStack();
                break;
            case "6":
                dbConnector.DeleteFlashcard();
                break;
            case "7":
                dbConnector.StudyFlashcards();
                break;
            case "8":
                dbConnector.ShowResults();
                break;
            case "9":
                dbConnector.ShowResultsByYear();
                break;
            case "10":
                dbConnector.ShowAverageByMonth();
                break;
            case "11":
                wantToExit = true;
                Console.WriteLine("Exiting the program...");
                break;
            default:
                Console.WriteLine("Invalid input. Please try again.");
                break;
        }
    }
    catch (Exception e)
    {
        Console.WriteLine(e.Message);
        continue;
    }
}