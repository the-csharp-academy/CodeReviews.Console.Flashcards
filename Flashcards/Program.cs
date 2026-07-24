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
        var sessionsRepository = new StudySessionsRepo(connectionFactory);

        var appView = new AppView();
        var stacksView = new StacksView();
        var flashcardsView = new FlashcardsView();
        var sessionsView = new StudySessionsView();

        var answerChecker = new AnswerChecker();

        var stackController = new StackController(stacksView, stacksRepository);
        var flashcardController = new FlashcardController(flashcardsView, flashcardsRepository, stacksRepository);
        var sessionController =
            new StudySessionController(
                sessionsView,
                stacksRepository,
                flashcardsRepository,
                sessionsRepository,
                answerChecker);
        var appController
            = new AppController(
                appView,
                stackController,
                flashcardController,
                sessionController);

        try
        {
            initializer.Initialize();
            appView.DisplayMessage("Database initialization complete. Ready to run.");
        }
        catch (Exception ex)
        {
            appView.DisplayMessage($"Database initialization failed: {ex.Message}");
            Environment.ExitCode = 1;
            return;
        }

        appController.Run();

    }

    private static IConfigurationRoot Setup()
        => new ConfigurationBuilder()
            .SetBasePath(AppContext.BaseDirectory)
            .AddJsonFile("Configuration/appsettings.json", optional: false, reloadOnChange: false)
            .Build();
}