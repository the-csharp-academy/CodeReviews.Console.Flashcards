using Kerem.Flashcards;
using Kerem.Flashcards.Infrastructure.Persistence;
using Kerem.Flashcards.UI;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

var builder = new ConfigurationBuilder()
    .SetBasePath(Directory.GetCurrentDirectory())
    .AddJsonFile("C:\\Users\\kerem\\RiderProjects\\CodeReviews.Console.Flashcards\\Kerem.Flashcards\\Kerem.Flashcards\\appsettings.json", optional: false);
    IConfiguration config = builder.Build();
    var connectionString = config.GetConnectionString("DefaultConnection");

    var serviceProvider = new ServiceCollection()
        .AddApplication(connectionString)
        .BuildServiceProvider();
    var connection = serviceProvider.GetRequiredService<DapperDbContext>().GetConnection();

    serviceProvider.GetRequiredService<DapperDbContext>().DbInit(connection);
    Dapper.DefaultTypeMap.MatchNamesWithUnderscores = true;

    serviceProvider.GetRequiredService<ConsoleMenu>().Menu();
