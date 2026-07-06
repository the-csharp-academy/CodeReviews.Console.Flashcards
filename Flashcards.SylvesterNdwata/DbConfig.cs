using Microsoft.Extensions.Configuration;

namespace silvermax.FlashCards;

internal class DbConfig
{
    private readonly string _connectionString;
    private readonly string _masterConnectionString;

    public DbConfig()
    {
        var config = new ConfigurationBuilder()
            .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
            .Build();

        _connectionString = config.GetConnectionString("DefaultConnection")
            ?? throw new Exception("Connection String not found");

        _masterConnectionString = config.GetConnectionString("masterConnectionString")
            ?? throw new Exception("Connection String not found");
    }

    public string ConnectionString => _connectionString;
    public string MasterConnectionString => _masterConnectionString;
}