using Dapper;
using Flashcards.Entities;
using Microsoft.Data.SqlClient;
using System.Data;

namespace Flashcards.Repositories
{
    public class CardStackRepository
    {
        private readonly string connectionString;
        private readonly string table = "stacks";

        public CardStackRepository(string connectionString)
        {
            this.connectionString = connectionString;
        }

        public bool CreateTable()
        {
            return Execute(db =>
            {
                var query =
                @$"IF NOT EXISTS 
                    (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'{table}') AND type in (N'U'))
                    CREATE TABLE {table}
                    (
                        Id INT IDENTITY(1, 1) PRIMARY KEY,
                        Name NVARCHAR(50) NOT NULL UNIQUE
                    );";
                return db.Execute(query) != 0;
            });
        }

        private bool Execute(Func<IDbConnection, bool> action)
        {
            using (IDbConnection db = new SqlConnection(connectionString))
            {
                return action(db);
            }
        }

        public bool CreateCardStack(CardStackEntity cardStack)
        {
            try
            {
                return Execute(db =>
                {
                    var query =
                    @$"INSERT INTO {table}(Name)
                        VALUES(@Name);";
                    return db.Execute(query, cardStack) > 0;
                });
            }
            catch (SqlException exception) when (exception.Number is 2601 or 2627)
            {
                return false;
            }
        }

        public CardStackEntity? GetCardStack(int id)
        {
            using (IDbConnection db = new SqlConnection(connectionString))
            {
                return db.Query<CardStackEntity>(
                    @$"SELECT * FROM {table} 
                        WHERE Id=@id", new { id })
                    .FirstOrDefault();
            }
        }

        public List<CardStackEntity> ReadCardStacks()
        {
            using (IDbConnection db = new SqlConnection(connectionString))
            {
                return db.Query<CardStackEntity>(
                    @$"SELECT * FROM {table}")
                    .ToList();
            }
        }

        public bool UpdateCardStack(CardStackEntity cardStack)
        {
            return Execute(db =>
            {
                var query =
                @$"UPDATE {table} 
                    SET Name=@Name WHERE Id=@Id";
                return db.Execute(query, cardStack) > 0;
            });
        }

        public bool DeleteCardStack(int id)
        {
            return Execute(db =>
            {
                var query =
                @$"DELETE FROM {table}
                    WHERE Id=@id";
                return db.Execute(query, new { id }) > 0;
            });
        }
    }
}
