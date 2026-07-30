using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Configuration;
using Dapper;
using Spectre.Console;


class DatabaseController
{
    static string connectionString = new ConfigurationBuilder()
    .AddJsonFile("appsettings.json")
    .Build()
    .GetConnectionString("DefaultConnection") ?? throw new InvalidOperationException("Connection string not found.");


    public static void ViewStackRecords()
    {
        GetStackRecords();
        Console.WriteLine();
        Console.ReadKey();

    }

    public static void ViewFlashcardRecords(int stackId)
    {
        GetFlashcardRecords(stackId);
        Console.WriteLine();
        Console.ReadKey();

    }
    public static void CreateStackTable()
    {
        using (var connection = new SqlConnection(connectionString))
        {
            connection.Execute(@"
            IF NOT EXISTS (SELECT * FROM sysobjects WHERE name='Stack' AND xtype='U')
            CREATE TABLE Stack (
                Id INT PRIMARY KEY IDENTITY(1,1),
                Name NVARCHAR(50) UNIQUE
            )");
        }
    }

    public static void CreateFlashcardTable()
    {
        using (var connection = new SqlConnection(connectionString))
        {
            connection.Execute(@"
            IF NOT EXISTS (SELECT * FROM sysobjects WHERE name='Flashcard' AND xtype='U')
            CREATE TABLE Flashcard (
                Id INT PRIMARY KEY IDENTITY(1,1),
                StackId INT NOT NULL,
                Front NVARCHAR(100)UNIQUE,
                Back NVARCHAR(100)UNIQUE
                FOREIGN KEY (StackId) REFERENCES Stack(Id) ON DELETE CASCADE    
            )");
        }
    }

    public static void CreateStudySessionsTable()
    {
        using (var connection = new SqlConnection(connectionString))
        {
            connection.Execute(@"
            IF NOT EXISTS (SELECT * FROM sysobjects WHERE name='StudySessions' AND xtype='U')
            CREATE TABLE StudySessions (
                Id INT PRIMARY KEY IDENTITY(1,1),
                StackId INT NOT NULL,
                Date DATETIME NOT NULL,
                Score INT,
                FOREIGN KEY (StackId) REFERENCES Stack(Id) ON DELETE CASCADE    
            )");
        }
    }

    public static void GetStackRecords()
    {
        Console.Clear();

        using (var connection = new SqlConnection(connectionString))
        {
            List<Stack> tabledata = connection.Query<Stack>("SELECT * FROM Stack").ToList();

            var table = new Table();

            table.AddColumn("Id");
            table.AddColumn("Name");

            foreach (var stack in tabledata)
            {
                table.AddRow(
                stack.Id.ToString(),
                stack.Name.ToString()
                );
            }

            AnsiConsole.Write(table);
        }

    }


    public static void InsertStack(string stackInput)
    {
        Console.Clear();

        using (var connection = new SqlConnection(connectionString))
        {
            connection.Execute("INSERT INTO Stack (Name) VALUES (@Name)", new { Name = stackInput });

            Console.WriteLine("Stack Inserted.\nPress any key to return to main menu.");
            Console.ReadKey();
        }
    }


    public static int GetStackIdByName(string name)
    {
        using (var connection = new SqlConnection(connectionString))
        {
            return connection.ExecuteScalar<int>(
                "SELECT Id FROM Stack WHERE Name = @Name",
                new { Name = name });
        }
    }


    public static void DeleteStackByName(string name)
    {
        using (var connection = new SqlConnection(connectionString))
        {
            int rowsAffected = connection.Execute("DELETE FROM Stack WHERE Name = @Name", new { Name = name });

            if (rowsAffected == 0)
                Console.WriteLine("No stack found with that name.");
            else
                Console.WriteLine("Stack deleted.");

            Console.ReadKey();
        }
    }


    public static void UpdateStackByName(string oldName, string newName)
    {
        using (var connection = new SqlConnection(connectionString))
        {
            int rowsAffected = connection.Execute("UPDATE Stack SET Name = @NewName WHERE Name = @OldName",
            new { OldName = oldName, NewName = newName });

            if (rowsAffected == 0)
                Console.WriteLine("No stack found with that name.");
            else
                Console.WriteLine("Stack Updated.");
        }

    }


    public static void GetFlashcardRecords(int stackId)
    {
        Console.Clear();

        using (var connection = new SqlConnection(connectionString))
        {
            List<FlashcardDTO> tabledata = connection.Query<FlashcardDTO>(
            @"SELECT * FROM Flashcard WHERE StackId = @StackId", new { StackId = stackId }).ToList();

            var table = new Table();

            table.AddColumn("Id");
            table.AddColumn("Front");
            table.AddColumn("Back");

            foreach (var card in tabledata)
            {
                table.AddRow(
                card.Id.ToString(),
                card.Front.ToString(),
                card.Back.ToString()

                );
            }

            AnsiConsole.Write(table);
        }

    }


    public static void InsertFlashcard(string front, string back, int stackId)
    {
        using (var connection = new SqlConnection(connectionString))
        {
            connection.Execute(@"INSERT INTO Flashcard (Front, Back, StackId)
            VALUES (@Front, @Back, @StackId)", new { Front = front, Back = back, StackId = stackId });

            Console.WriteLine("Flashcard Inserted.\nPress any key to return to main menu.");
            Console.ReadKey();
        }

    }

    public static void DeleteFlashcard(int input, int stackId)
    {
        using (var connection = new SqlConnection(connectionString))
        {
            int rowsAffected = connection.Execute("DELETE FROM Flashcard WHERE Id = @Id", new { Id = input });

            if (rowsAffected == 0)
            {
                Console.WriteLine("Row not found.\nPlease try again.");
                Console.ReadLine();
            }
            else
            {
                Console.WriteLine("Flashcard Deleted.\nPress any key to return to main menu.");
                Console.ReadKey();
            }
        }

    }


    public static void UpdateFlashcard(int updateId, int stackId, string front, string back)
    {

        using (var connection = new SqlConnection(connectionString))
        {

            int rowsAffected = connection.Execute(@"UPDATE Flashcard SET Front = @Front, Back = @Back WHERE Id = @Id",
            new { Id = updateId, Front = front, Back = back });

            if (rowsAffected == 0)
            {
                Console.WriteLine("Row not found.\nPlease try again.");
                Console.ReadLine();
            }
            else
            {
                Console.WriteLine("Flashcard Updated.\nPress any key to return to main menu.");
                Console.ReadKey();
            }
        }

    }

    public static void InsertSession(int stackId, string Score)
    {

        using (var connection = new SqlConnection(connectionString))
        {
            connection.Execute(@"INSERT INTO StudySessions (StackId, Score, Date )
            VALUES (@StackId, @Score, @Date)", new { StackId = stackId, Date = DateTime.Now, Score = Score });

        }

    }

    public static void PreviousSessions()
    {
        using (var connection = new SqlConnection(connectionString))
        {
            List<studySession> sessions = connection.Query<studySession>(
            "SELECT * FROM StudySessions").ToList();

            var table = new Table();
            table.AddColumn("Id");
            table.AddColumn("Score");
            table.AddColumn("Date");

            foreach (var sesh in sessions)
            {
                table.AddRow(
                sesh.Id.ToString(),
                sesh.Score.ToString(),
                sesh.Date.ToString());
            }

            AnsiConsole.Write(table);
        }

        Console.WriteLine("Press any key to return.");
        Console.ReadKey();
    }

    public static List<FlashcardDTO> GetFlashcards(int stackId)
    {
        using (var connection = new SqlConnection(connectionString))
        {
            return connection.Query<FlashcardDTO>(
                "SELECT Front, Back FROM Flashcard WHERE StackId = @StackId",
                new { StackId = stackId }).ToList();
        }
    }

//     public static bool StackNameExists(string name)
// {
//     var controller = new DatabaseController();

//     using (var connection = new SqlConnection(connectionString))
//     {
//         int count = connection.ExecuteScalar<int>(
//             "SELECT COUNT(*) FROM Stack WHERE Name = @Name",
//             new { Name = name });
//         return count > 0;
//     }
// }

}

