using FlashcardsApp.Repositories;
using Microsoft.Extensions.Configuration;

namespace FlashcardsApp
{
    public class Program
    {
        public static void Main(string[] args)
        {
            IConfiguration configuration = new ConfigurationBuilder()
                .SetBasePath(AppContext.BaseDirectory)
                .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
                .Build();

            string connectionString = configuration.GetConnectionString("DefaultConnection")
                ?? throw new InvalidOperationException("Connection string 'DefaultConnection' not found in appsettings.json.");

            InitDatabase.CreateDatabase(configuration);
            Menu menu = new Menu(connectionString);
        }
    }
}