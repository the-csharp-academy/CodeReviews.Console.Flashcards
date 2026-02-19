using Flashcards._0lcm.CRUDController;
using Flashcards._0lcm.Interfaces;
using Flashcards._0lcm.Logging;
using Flashcards._0lcm.Services;
using Flashcards._0lcm.UserInterface;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;

namespace Flashcards._0lcm;

internal class Program
{
    internal static readonly IConfiguration Configuration = new ConfigurationBuilder()
        .SetBasePath(AppContext.BaseDirectory)
        .AddJsonFile("appSettings.json", false, true)
        .Build();

    private static readonly ILogger Logger = AppLogger.CreateLogger<Program>();

    internal static void Main()
    {
        try
        {
            LocalDbController.CreateDatabase();
            LocalDbController.CreateTables();

            IStackService stackService = new StackService();
            var stackUi = new StackUi(stackService);

            IFlashcardService flashcardService = new FlashcardService();
            var flashcardUi = new FlashcardUi(flashcardService, stackService);

            IStudyService studyService = new StudyService();
            var studyUi = new StudyUi(studyService, stackService, flashcardService);

            var consoleUi = new ConsoleUi(stackUi, flashcardUi, studyUi);
            consoleUi.MainMenu();
        }
        catch (Exception ex)
        {
            Logger.LogCritical(ex, "Error caught by Program.cs");
            System.Environment.Exit(1);
        }
    }
}