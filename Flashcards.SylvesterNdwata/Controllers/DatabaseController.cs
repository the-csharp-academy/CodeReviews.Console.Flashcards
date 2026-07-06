using Microsoft.Data.SqlClient;
using Dapper;

namespace silvermax.FlashCards.Controllers;

internal class DatabaseController
{
    private readonly DbConfig dbConfig = new();
    public void InitTables()
    {
        using (var masterCon = new SqlConnection(dbConfig.MasterConnectionString))
        {
            masterCon.Open();

            masterCon.Execute("IF NOT EXISTS (SELECT * FROM sys.databases WHERE name = 'flashcardDB') CREATE DATABASE flashcardDB");
        }

        using (var connection = new SqlConnection(dbConfig.ConnectionString))
        {
            connection.Open();

            string createStackTableSql = @"
                    IF NOT EXISTS (SELECT * FROM sys.tables WHERE name = 'Stacks')
                    BEGIN
                        CREATE TABLE Stacks (
                            Id INT PRIMARY KEY IDENTITY(1,1),
                            StackName NVARCHAR(50) NOT NULL
                        )
                    END";

            string createCardTableSql = @"
                    IF NOT EXISTS (SELECT * FROM sys.tables WHERE name = 'Flashcards')
                    BEGIN
                        CREATE TABLE Flashcards (
                            Id INT PRIMARY KEY IDENTITY(1,1),
                            StackId INT NOT NULL,
                            Word NVARCHAR(50),
                            Translation NVARCHAR(50)
                            CONSTRAINT FK_Flashcards_Stacks FOREIGN KEY (StackId)
                                REFERENCES Stacks(Id) ON DELETE CASCADE
                        )
                    END";

            string createSessionTableSql = @"
                    IF NOT EXISTS (SELECT * FROM sys.tables WHERE name = 'Sessions')
                    BEGIN
                        CREATE TABLE Sessions (
                            Id INT PRIMARY KEY IDENTITY(1,1),
                            Date DATETIME2 NOT NULL,
                            Score INT NOT NULL
                        )
                    END";
            connection.Execute(createStackTableSql);
            connection.Execute(createCardTableSql);
            connection.Execute(createSessionTableSql);
        }
    }
}
