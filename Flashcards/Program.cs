using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
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
        var appView = new AppView();
        var appController = new AppController(appView);
        try
        {
            initializer.Initialize();
            appView.DisplayMessage("[green]Database initialization complete. Ready to run.[/]");
        }
        catch (Exception ex)
        {
            appView.DisplayMessage($"[red]Database initialization failed: {ex.Message}[/]");
            throw;
        }

        appController.Run();

    }

    private static IConfigurationRoot Setup()
        => new ConfigurationBuilder()
            .SetBasePath(AppContext.BaseDirectory)
            .AddJsonFile("Configuration/appsettings.json", optional: false, reloadOnChange: false)
            .Build();
}