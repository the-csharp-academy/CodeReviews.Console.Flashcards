using Flashcards.TerrenceLGee.Data;
using Flashcards.TerrenceLGee.Data.Interfaces;
using Flashcards.TerrenceLGee.Data.Repositories;
using Flashcards.TerrenceLGee.FlashcardUI;
using Flashcards.TerrenceLGee.FlashcardUI.Interfaces;
using Flashcards.TerrenceLGee.Services;
using Flashcards.TerrenceLGee.Services.Interfaces;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Serilog;

try
{
    LoggingSetup();
    await Startup();
}
catch (Exception ex)
{
    Console.WriteLine($"There was an unexpected error starting this program: {ex.Message}");
}

return;

async Task Startup()
{
    IConfiguration configuration = new ConfigurationBuilder()
        .SetBasePath(Directory.GetCurrentDirectory())
        .AddJsonFile("appsettings.json")
        .Build();

    var connectionStringValue = configuration.GetConnectionString("DefaultConnection")
                                ?? throw new InvalidOperationException("Unable to retrieve connection string");

    var connectionString = new ConnectionString(connectionStringValue);

    var services = new ServiceCollection()
        .AddLogging(loggingBuilder => loggingBuilder.AddSerilog(dispose: true))
        .AddSingleton(connectionString)
        .AddScoped<IDatabaseInitializer, DatabaseInitializer>()
        .AddScoped<IStudyStackRepository, StudyStackRepository>()
        .AddScoped<IFlashcardRepository, FlashcardRepository>()
        .AddScoped<IStudySessionRepository, StudySessionRepository>()
        .AddScoped<ISessionFlashcardRepository, SessionFlashcardRepository>()
        .AddScoped<IStudyStackService, StudyStackService>()
        .AddScoped<IFlashcardService, FlashcardService>()
        .AddScoped<IStudySessionService, StudySessionService>()
        .AddScoped<ISessionFlashcardService, SessionFlashcardService>()
        .AddScoped<IStudyStackUi, StudyStackUi>()
        .AddScoped<IFlashcardUi, FlashcardUi>()
        .AddScoped<IStudySessionUi, StudySessionUi>()
        .AddScoped<IViewUi, ViewUi>();

    var serviceProvider = services.BuildServiceProvider();

    var database = serviceProvider.GetRequiredService<IDatabaseInitializer>();

    await database.InitializeDatabaseAsync();

    var stackUi = serviceProvider.GetRequiredService<IStudyStackUi>();
    var flashcardUi = serviceProvider.GetRequiredService<IFlashcardUi>();
    var sessionUi = serviceProvider.GetRequiredService<IStudySessionUi>();
    var viewUi = serviceProvider.GetRequiredService<IViewUi>();
    var stackService = serviceProvider.GetRequiredService<IStudyStackService>();

    var app = new FlashcardApp(
        stackUi,
        flashcardUi,
        sessionUi,
        viewUi,
        stackService);

    await app.RunAsync();
}

void LoggingSetup()
{
    var loggingDirectory = Path.Combine(Directory.GetCurrentDirectory(), "Logs");
    Directory.CreateDirectory(loggingDirectory);
    var filePath = Path.Combine(loggingDirectory, "cdtkr-.txt");
    var outputTemplate = "{Timestamp:yyyy-MM-dd HH:mm:ss} [{Level:u3}] {Message:lj}{NewLine}{Exception}";

    Log.Logger = new LoggerConfiguration()
        .MinimumLevel.Information()
        .WriteTo.File(
            path: filePath,
            rollingInterval: RollingInterval.Day,
            outputTemplate: outputTemplate)
        .CreateLogger();
}
