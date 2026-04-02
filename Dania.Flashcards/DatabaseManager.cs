using Dapper;
using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Configuration;
using Flashcards.Models;

namespace Flashcards
{
    internal class DatabaseManager
    {
        //Appsetting.json config connection
        internal string GetConnectionString()
        {
            IConfiguration config = new ConfigurationBuilder().AddJsonFile("appsettings.json").Build();
            string connectionString = config.GetConnectionString("DefaultConnection");

            return connectionString;

        }

        internal bool TestConnection()
        {
            try
            {
                using (var connection = new SqlConnection(GetConnectionString()))
                {
                    connection.Open();
                    return true; // Connection successful
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Database connection failed: {ex.Message}");
                return false; // Connection failed
            }
        }

        internal void CreateTable()
        {
            using (var connection = new SqlConnection(GetConnectionString()))
            {
                var sql = @"IF NOT EXISTS (SELECT * FROM sys.tables WHERE name = 'Stacks')
                            BEGIN
                                CREATE TABLE Stacks (
                                    Id INT PRIMARY KEY IDENTITY(1,1),
                                    Name NVARCHAR(255) UNIQUE NOT NULL
                                );
                            END
                            
                            IF NOT EXISTS (SELECT * FROM sys.tables WHERE name = 'Flashcards')
                            BEGIN
                                CREATE TABLE Flashcards (
                                    Id INT PRIMARY KEY IDENTITY(1,1),
                                    StackId INT NOT NULL,
                                    Question NVARCHAR(MAX) NOT NULL,
                                    Answer NVARCHAR(MAX) NOT NULL,
                                    FOREIGN KEY (StackId) REFERENCES Stacks(Id) ON DELETE CASCADE
                                );
                            END
                            
                            IF NOT EXISTS (SELECT * FROM sys.tables WHERE name = 'StudySessions')
                            BEGIN
                                CREATE TABLE StudySessions(
                                    Id INT PRIMARY KEY IDENTITY(1,1),
                                    StackId INT NOT NULL,
                                    Date DATETIME NOT NULL,
                                    Score INT NOT NULL,
                                    TotalQuestions INT NOT NULL,
                                    FOREIGN KEY (StackId) REFERENCES Stacks(Id) ON DELETE CASCADE
                                );
                            END";

                connection.Execute(sql);
            }
        }

        internal void CreateStack(string stackName)
        {
            using (var connection = new SqlConnection(GetConnectionString()))
            {
                var sql = "INSERT INTO Stacks (Name) VALUES (@Name)";

                connection.Execute(sql, new { Name = stackName });
            }
        }

        internal List<Stack> GetStacks()
        {
            using (var connection = new SqlConnection(GetConnectionString()))
            {
                var sql = "SELECT * FROM Stacks ORDER BY Id ASC";

                return connection.Query<Stack>(sql).ToList();
            }

        }

        internal void DeleteStack(int stackId)
        {
            using (var connection = new SqlConnection(GetConnectionString()))
            {
                var sql = "DELETE FROM Stacks WHERE Id = @Id";
                connection.Execute(sql, new { Id = stackId });
            }
        }

        internal void CreateFlashcard(int stackId, string question, string answer)
        {
            using (var connection = new SqlConnection(GetConnectionString()))
            {
                var sql = "INSERT INTO Flashcards (StackId, Question, Answer) VALUES (@StackId, @Question, @Answer)";

                connection.Execute(sql, new { StackId = stackId, Question = question, Answer = answer });
            }

        }

        internal List<FlashcardDto> GetFlashcardsByStackId(int stackId)
        {
            using (var connection = new SqlConnection(GetConnectionString()))
            {
                var sql = @"SELECT Id, 
                                   ROW_NUMBER() OVER (ORDER BY Id ASC) AS DisplayId, 
                                   Question, 
                                   Answer 
                            FROM Flashcards WHERE StackId = @StackId";

                return connection.Query<FlashcardDto>(sql, new { StackId = stackId }).ToList();
            }
        }

        internal void UpdateFlashcard(int flashcardId, string question, string answer)
        {
            using (var connection = new SqlConnection(GetConnectionString()))
            {
                var sql = "UPDATE Flashcards SET Question = @Question, Answer = @Answer WHERE Id = @Id";
                connection.Execute(sql, new { Id = flashcardId, Question = question, Answer = answer });
            }
        }

        internal void DeleteFlashcard(int flashcardId)
        {
            using (var connection = new SqlConnection(GetConnectionString()))
            {
                var sql = "DELETE FROM Flashcards WHERE Id = @Id";
                connection.Execute(sql, new { Id = flashcardId });
            }
        }

        internal bool CheckStackExist(string stackname)
        {
            using (var connection = new SqlConnection(GetConnectionString()))
            {
                var sql = "SELECT COUNT(1) FROM Stacks WHERE Name = @Name";

                int count = connection.ExecuteScalar<int>(sql, new { Name = stackname });
                return count > 0;
            }
        }

        internal bool CheckStackExist(int id)
        {
            using (var connection = new SqlConnection(GetConnectionString()))
            { 
                var sql = "SELECT COUNT(1) FROM Stacks WHERE Id = @Id";
                
                int count = connection.ExecuteScalar<int>(sql, new { Id = id });
                return count > 0;
            }
        }

        internal void CreateStudySession(int stackId, int score, int totalQuestions)
        {
            using (var connection = new SqlConnection(GetConnectionString()))
            {
                var sql = "INSERT INTO StudySessions (StackId, Date, Score, TotalQuestions) VALUES (@StackId, @Date, @Score, @TotalQuestions)";
                connection.Execute(sql, new { StackId = stackId, Date = DateTime.Now, Score = score, TotalQuestions = totalQuestions });
            }
        }

        internal List<StudySession> GetStudySessions()
        {
            using (var connection = new SqlConnection(GetConnectionString()))
            {
                var sql = @"SELECT
                                Stacks.Name AS StackName,
                                StudySessions.Date,
                                StudySessions.Score,
                                StudySessions.TotalQuestions
                            FROM StudySessions JOIN Stacks ON StudySessions.StackId = Stacks.Id";

                return connection.Query<StudySession>(sql).ToList();
            }

        }
    }
}
