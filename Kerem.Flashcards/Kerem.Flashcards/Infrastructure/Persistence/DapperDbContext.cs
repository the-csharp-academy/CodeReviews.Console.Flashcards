using System.ComponentModel;
using System.Data;
using Dapper;
using Microsoft.Data.SqlClient;

namespace Kerem.Flashcards.Infrastructure.Persistence;


    public class DapperDbContext : IDisposable
    {
        private readonly string _connectionString;
        private IDbConnection _connection;

        public DapperDbContext(string connectionString)
        {
            _connectionString = connectionString;
        }

        public IDbConnection GetConnection()
        {
            if (_connection == null || _connection.State != ConnectionState.Open)
            {
                _connection = new SqlConnection(_connectionString);
                _connection.Open();
            }
            Dapper.DefaultTypeMap.MatchNamesWithUnderscores = true;

            return _connection;
        }

        public void Dispose()
        {
            if (_connection != null && _connection.State == ConnectionState.Open)
            {
                _connection.Close();
                _connection.Dispose();
            }
        }

        public void DbInit(IDbConnection connection)
        {
            var sql = """
            IF (OBJECT_ID('Stacks') IS NOT NULL )
                BEGIN
                    PRINT 'Table with the same name available'
                END
            ELSE
                BEGIN
            PRINT 'No Table available with this name. Let us create a new table'
            CREATE TABLE Stacks (
                Id INT PRIMARY KEY IDENTITY(1,1),
                Name NVARCHAR(255) UNIQUE NOT NULL);        
            CREATE TABLE Flashcards (
                Id INT PRIMARY KEY IDENTITY(1,1),
                Name NVARCHAR(255) NOT NULL,
                StackId INT,
                FOREIGN KEY (StackId) REFERENCES Stacks(Id) ON DELETE CASCADE);
            CREATE TABLE StudySessions (
                Id INT PRIMARY KEY IDENTITY(1,1),
                CreatedAt DATETIME NOT NULL,
                Score INT NOT NULL,
                StackId INT,
                FOREIGN KEY (StackId) REFERENCES Stacks(Id)  ON DELETE CASCADE);
                END
            """;
            connection.Execute(sql);
            
        }
    }