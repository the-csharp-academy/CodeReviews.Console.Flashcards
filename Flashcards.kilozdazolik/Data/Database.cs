using Dapper;
using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Configuration;

namespace Flashcards.kilozdazolik.Data;

internal static class Database
{
    private static readonly IConfiguration Config;

    static Database()
    {
        Config = new ConfigurationBuilder().SetBasePath(Directory.GetCurrentDirectory()).AddJsonFile("appsettings.json")
            .Build();
    }

    public static SqlConnection GetConnection()
    {
        return new SqlConnection(Config.GetConnectionString("DefaultConnection"));
    }

    public static void CreateDatabase()
    {
        using (var conn = GetConnection())
        {
            string createTable =
                "IF NOT EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'dbo.stacks') AND type = 'U')\nBEGIN\n    CREATE TABLE dbo.stacks (\n        stack_id INT IDENTITY (1,1) PRIMARY KEY,\n        name VARCHAR(50) NOT NULL UNIQUE\n    );\nEND\n\nIF NOT EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'dbo.flashcards') AND type = 'U')\nBEGIN\n    CREATE TABLE dbo.flashcards (\n        card_id INT IDENTITY (1,1) PRIMARY KEY,\n        front VARCHAR(50) NOT NULL, \n        back VARCHAR(50) NOT NULL,\n        stack_id INT NOT NULL, \n        FOREIGN KEY (stack_id)\n            REFERENCES dbo.stacks(stack_id)\n            ON DELETE CASCADE ON UPDATE CASCADE\n    );\nEND\n\nIF NOT EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'dbo.sessions') AND type = 'U')\nBEGIN\n    CREATE TABLE dbo.sessions (\n        session_id INT IDENTITY (1,1) PRIMARY KEY,\n        date DATETIME NOT NULL,\n        score INT NOT NULL,\n        stack_id INT NOT NULL, \n        FOREIGN KEY (stack_id)\n            REFERENCES dbo.stacks(stack_id)\n            ON DELETE CASCADE ON UPDATE CASCADE\n    );\nEND";
            conn.Execute(createTable);
        }
    }
    
    public static void CreateDummyData()
    {
        using (var conn = Database.GetConnection())
        {
            conn.Execute("INSERT INTO dbo.stacks (name) VALUES ('HTML Basics'); SELECT @StackId_HTML = SCOPE_IDENTITY(); INSERT INTO dbo.stacks (name) VALUES ('SQL Fundamentals'); SELECT @StackId_SQL = SCOPE_IDENTITY();", new { StackId_HTML = 0, StackId_SQL = 0 });
            
            int htmlId = conn.QuerySingle<int>("SELECT MAX(stack_id) FROM dbo.stacks WHERE name = 'HTML Basics'");
            int sqlId = conn.QuerySingle<int>("SELECT MAX(stack_id) FROM dbo.stacks WHERE name = 'SQL Fundamentals'");

            conn.Execute("INSERT INTO dbo.flashcards (stack_id, front, back) VALUES (@HtmlId, 'What is HTML?', 'HyperText Markup Language'), (@HtmlId, 'Root tag?', '<html>'), (@SqlId, 'Data retrieve statement?', 'SELECT'), (@SqlId, 'Unique key?', 'PRIMARY KEY');", new { HtmlId = htmlId, SqlId = sqlId });
            conn.Execute("INSERT INTO dbo.sessions (stack_id, date, score) VALUES (@HtmlId, DATEADD(day, -1, GETDATE()), 4), (@SqlId, GETDATE(), 3), (@HtmlId, DATEADD(day, -5, GETDATE()), 2);", new { HtmlId = htmlId, SqlId = sqlId });
        }
    }
}