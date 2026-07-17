using Microsoft.Extensions.Configuration;
using Spectre.Console;

namespace CodeReviews.Console.Flashcards;
class Program
{
    static void Main(string[] args)
    {
        var config = Setup();

        string connectionString = config.GetConnectionString("DatabaseConnection")
            ?? throw new InvalidOperationException("The DatabaseConnection connection string is missing.");

        var connectionFactory = new DatabaseConnectionFactory(connectionString);
        var initializer = new DatabaseInitializer(connectionFactory);

        try
        {
            initializer.Initialize();
            AnsiConsole.MarkupLine("[green]Database initialization complete. Ready to run.[/]");
        }
        catch (Exception ex)
        {
            AnsiConsole.MarkupLine($"[red]Database initialization failed: {ex.Message}[/]");
            throw;
        }
    }

    private static IConfigurationRoot Setup()
        => new ConfigurationBuilder()
            .SetBasePath(AppContext.BaseDirectory)
            .AddJsonFile("Configuration/appsettings.json", optional: false, reloadOnChange: false)
            .Build();
}