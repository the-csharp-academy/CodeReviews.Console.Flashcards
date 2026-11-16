using Flashcards.Extensions;
using Flashcards.UI;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

var builder = Host.CreateApplicationBuilder(args);

builder.Services.AddCustomServices();

using var host = builder.Build();

using var scope = host.Services.CreateScope();

var mainMenu = host.Services.GetRequiredService<MainMenu>();

await mainMenu.Show();
