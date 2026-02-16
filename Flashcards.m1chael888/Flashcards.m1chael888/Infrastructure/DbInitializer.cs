using Dapper;
using Microsoft.Data.SqlClient;

namespace Flashcards.m1chael888.Infrastructure;

public interface IDbInitializer
{
    void Initialize();
}
public class DbInitializer : IDbInitializer
{
    private readonly string _connectionString;

    public DbInitializer(string connectionString)
    {
        _connectionString = connectionString;
    }

    public void Initialize()
    {
        var sql = @"IF NOT EXISTS (SELECT * FROM sys.tables WHERE name = 'Stacks')
                        BEGIN
                            CREATE TABLE Stacks (
                                StackId INTEGER IDENTITY(1,1) PRIMARY KEY,
                                Name TEXT NOT NULL
                            );
                        END

                        IF NOT EXISTS (SELECT * FROM sys.tables WHERE name = 'Cards')
                        BEGIN
                            CREATE TABLE Cards (
                                CardId INTEGER IDENTITY(1,1) PRIMARY KEY,
                                Front TEXT NOT NULL,
                                Back TEXT NOT NULL,
                                StackId INTEGER FOREIGN KEY REFERENCES Stacks(StackId) 
                            );
                        END

                        IF NOT EXISTS (SELECT * FROM sys.tables WHERE name = 'Sessions')
                        BEGIN
                            CREATE TABLE Sessions (
                                SessionId INTEGER IDENTITY(1,1) PRIMARY KEY,
                                Date TEXT NOT NULL,
                                Score TEXT NOT NULL,
                                StackId INTEGER FOREIGN KEY REFERENCES Stacks(StackId) 
                            );
                        END";

        using (var connection = new SqlConnection(_connectionString))
        {
            connection.Execute(sql);
        }
    }
}
