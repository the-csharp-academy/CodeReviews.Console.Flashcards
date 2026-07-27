using System;
using Microsoft.Data.SqlClient;

namespace FlashcardsApp.Repositories
{
    internal class InitDatabase
    {
        private static string CreateDatabaseQuery()
        {
            return @"
                IF NOT EXISTS (SELECT * FROM sys.databases WHERE name = 'Flashcard')
                BEGIN
                    CREATE DATABASE Flashcard;
                END
            ";
        }

        private static string CreateFlashcardTableQuery()
        {
            return @"
                IF OBJECT_ID(N'dbo.Flashcards', 'U') IS NULL
                BEGIN
                    CREATE TABLE dbo.Flashcards (
                        Id INT PRIMARY KEY IDENTITY(1,1),
                        StackID INT NOT NULL,
                        FRONT VARCHAR(255),
                        BACK VARCHAR(255),
                        CONSTRAINT FK_Flashcards_Stacks FOREIGN KEY (StackId) REFERENCES dbo.Stacks(Id) ON DELETE CASCADE
                    );
                END
            ";
        }

        private static string CreateStackTableQuery()
        {
            return @"
                IF OBJECT_ID(N'dbo.Stacks', 'U') IS NULL
                BEGIN
                    CREATE TABLE dbo.Stacks (
                        Id INT PRIMARY KEY IDENTITY(1,1),
                        Name VARCHAR(100)
                    );
                END
            ";
        }

        private static string CreateSessionTableQuery()
        {
            return @"
                IF OBJECT_ID(N'dbo.Sessions', 'U') IS NULL
                BEGIN
                    CREATE TABLE dbo.Sessions (
                        Id INT PRIMARY KEY IDENTITY(1,1),
                        StackID INT NOT NULL,
                        Date DATETIME,
                        Score INT,
                        CONSTRAINT FK_Sessions_Stacks FOREIGN KEY (StackId) REFERENCES dbo.Stacks(Id) ON DELETE CASCADE
                    );
                END
            ";
        }

        private static void CreateTables()
        {
            string strCreateTableFlashcard = CreateFlashcardTableQuery();
            string strCreateTableStack = CreateStackTableQuery();
            string strCreateTableSession = CreateSessionTableQuery();

            string connectionString = "Server=.;Database=Flashcard;Trusted_Connection=True;TrustServerCertificate=True;";
            try
            {
                using (var conn = new SqlConnection(connectionString))
                {
                    conn.Open();
                    new SqlCommand(strCreateTableStack, conn).ExecuteNonQuery();
                    new SqlCommand(strCreateTableSession, conn).ExecuteNonQuery();
                    new SqlCommand(strCreateTableFlashcard, conn).ExecuteNonQuery();
                    conn.Close();
                }
            }
            catch (SqlException ex)
            {
                Console.WriteLine($"Failed to create database: {ex.Message}");
            }
        }

        internal static void CreateDatabase()
        {
            string strCreateDatabase = CreateDatabaseQuery();

            string connectionString = "Server=.;Trusted_Connection=True;TrustServerCertificate=True;";

            try
            {
                using (var conn = new SqlConnection(connectionString))
                {
                    var command = new SqlCommand(strCreateDatabase, conn);
                    conn.Open();
                    command.ExecuteNonQuery();
                    conn.Close();
                }
            }
            catch (SqlException ex)
            {
                Console.WriteLine($"Failed to create database: {ex.Message}");
            }

            CreateTables();
        }
    }
}
