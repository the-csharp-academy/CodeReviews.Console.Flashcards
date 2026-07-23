using Microsoft.Extensions.Configuration;

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
        var stacksRepository = new StacksRepo(connectionFactory);
        var flashcardsRepository = new FlashcardsRepo(connectionFactory);
        var appView = new AppView();
        var stacksView = new StacksView();
        var flashcardsView = new FlashcardsView();
        var stackController = new StackController(stacksView, stacksRepository);
        var flashcardController = new FlashcardController(flashcardsView, flashcardsRepository, stacksRepository);
        var appController = new AppController(appView, stackController, flashcardController);
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