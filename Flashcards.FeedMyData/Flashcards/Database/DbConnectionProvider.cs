using Microsoft.Data.SqlClient;
using Spectre.Console;

namespace Flashcards;

public sealed class DbConnectionProvider
{
    private readonly string _connectionString;

    internal DbConnectionProvider(string connectionString)
    {
        _connectionString = connectionString;
    }

    internal SqlConnection GetConnection()
    {
        return new SqlConnection(_connectionString);
    }

    internal T Execute<T>(Func<SqlConnection, T> action)
    {
        while (true)
        {
            using var connection = GetConnection();
            try
            {
                connection.Open();
            }
            // catching SQL Server being unavailable.
            catch (SqlException ex) when (ex.ErrorCode == -2146232060)
            {
                AnsiConsole.MarkupLine($"[red]SQL Server is likely unavailable. Check that your server is running.[/]");
                if (AnsiConsole.Confirm("Do you want to retry?"))
                    continue;

                throw new DatabaseAccessException("Impossible to connect.", ex);
            }
            return action(connection);
        }
    }
}