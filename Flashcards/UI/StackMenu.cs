using Flashcards.Services;
using Spectre.Console;

namespace Flashcards.UI
{
    internal class StackMenu
    {
        private readonly StackService _stackService;
        private List<DTOs.StackDTO> _stacksList = new();

        public StackMenu(StackService stackService)
        {
            _stackService = stackService;
        }
        internal async Task Show()
        {
            bool show = true;

            while (show)
            {
                AnsiConsole.Clear();
                _stacksList = await _stackService.GetAllStacksDTOAsync(null);

                var table = TableBuilder.ShowStackTable(_stacksList);

                switch (_stacksList.Count > 0)
                {
                    case true:
                        AnsiConsole.Write(table);
                        break;
                    case false:
                        AnsiConsole.MarkupLine("[red]No Stacks found in the database![/]");
                        break;
                }

                var choice = AnsiConsole.Prompt(new SelectionPrompt<string>()
                    .Title("What do you want to do?")
                    .AddChoices("Create", "Update", "Delete", "Back"));

                switch (choice)
                {
                    case "Create":
                        await CreateStack();
                        break;
                    case "Update":
                        {
                            switch (_stacksList.Count > 0)
                            {
                                case true:
                                    await UpdateStack();
                                    break;
                                case false:
                                    AnsiConsole.MarkupLine("[red]Nothing to update\n" +
                                        "Press any key to continue...[/]");
                                    Console.ReadKey();
                                    break;
                            }
                        }
                        break;
                    case "Delete":
                        {
                            switch (_stacksList.Count > 0)
                            {
                                case true:
                                    await DeleteStack();
                                    break;
                                case false:
                                    AnsiConsole.MarkupLine("[red]Nothing to delete\n" +
                                        "Press any key to continue...[/]");
                                    Console.ReadKey();
                                    break;
                            }
                        }
                        break;
                    case "Back":
                        show = false;
                        break;
                }
            }
        }

        internal async Task CreateStack()
        {
            var name = AnsiConsole.Prompt(new TextPrompt<string>("Please enter a stack name")
                .Validate(input => !string.IsNullOrWhiteSpace(input), "[red]Stack name cannot be empty, please enter a stack name.[/]"));

            await _stackService.CreateStackAsync(name);
        }

        internal async Task DeleteStack()
        {
            var choice = AnsiConsole.Prompt(new SelectionPrompt<DTOs.StackDTO>()
                .Title("Please select a stack to delete")
                .UseConverter(x =>
                {
                    return $"{x.Id} - {x.Name}";
                })
                .AddChoices(_stacksList));

            await _stackService.DeleteStackAsync(choice);
        }

        internal async Task UpdateStack()
        {
            var choice = AnsiConsole.Prompt(new SelectionPrompt<DTOs.StackDTO>()
                .Title("Please select a stack to update:")
                .UseConverter(x =>
                {
                    return $"{x.Id} - {x.Name}";
                })
                .AddChoices(_stacksList));

            var newName = AnsiConsole.Prompt(new TextPrompt<string>($"The current stack name is: {choice.Name}" +
                $"\nPlease enter a new name:")
                .AllowEmpty()
                .Validate(input =>
                {
                    if (string.IsNullOrWhiteSpace(input))
                    {
                        return ValidationResult.Error("[red]Stack name cannot be empty, please enter a stack name.[/]");
                    }
                    if (input == choice.Name)
                    {
                        return ValidationResult.Error("[red]New name cannot be the same as old name, please enter a new stack name[/]");
                    }

                    return ValidationResult.Success();
                }));

            await _stackService.UpdateStackAsync(choice, newName);
        }
    }
}
