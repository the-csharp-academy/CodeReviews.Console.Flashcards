using Dapper;
using Microsoft.Data.SqlClient;
using silvermax.FlashCards.Dtos;
using silvermax.FlashCards.Models;
using Spectre.Console;
using static silvermax.FlashCards.Enums;

namespace silvermax.FlashCards.Controllers;

internal class StacksController
{
    private readonly DatabaseHelper db = new();
    UserInput input = new();
    public void Start()
    {
        var choice = AnsiConsole.Prompt(
        new SelectionPrompt<ManageStack>()
        .Title("Choose an action to do on stacks")
        .UseConverter(e => Enums.ToDisplayName(e))
        .AddChoices(Enum.GetValues<ManageStack>()));

        switch (choice)
        {
            case ManageStack.AddStack:
                AddStack();
                break;

            case ManageStack.EditStack:
                EditStack();
                break;

            case ManageStack.DeleteStack:
                DeleteStack();
                break;

            case ManageStack.CreateFlashcardInStack:
                AddFlashCardToStack();
                break;

            case ManageStack.ShowAllFlashcardsInStack:
                ShowAllCardsInStack();
                break;

            case ManageStack.Back:
                return;
        }
    }

    private void ShowAllCardsInStack()
    {
        var currentStack = ChooseStack();

        using (var connection = db.GetConnection())
        {

            var table = new Table();
            table.Border = TableBorder.Rounded;

            table.AddColumn("[yellow]Id[/]");
            table.AddColumn("[yellow]Front[/]");
            table.AddColumn("[yellow]Back[/]");

            int currentStackID = connection.QuerySingle<int>("SELECT Id FROM Stacks WHERE StackName = @StackName", new { StackName = currentStack });

            var cards = connection.Query<FlashCardResponseDto>("SELECT * FROM Flashcards WHERE StackId = @Id", new { Id = currentStackID }).ToList();

            if (cards.Count == 0)
            {
                AnsiConsole.MarkupLine("[red]No rows found in the database[/]");
            }
            else
            {
                int displayId = 1;
                foreach (var card in cards)
                {
                    table.AddRow(
                        (displayId++).ToString(),
                        $"[purple]{card.Word}[/]",
                        $"[purple]{card.Translation}[/]");
                }

                AnsiConsole.Write(table);
            }
        }

        UIHelper.PressAnyKey();
    }

    public string ChooseStack()
    {
        var stack = AnsiConsole.Ask<string>("Please write the name of the stack you want to work on: ").ToLower();

        AnsiConsole.MarkupLine($"Current working stack: {stack}");

        return stack;
    }

    private void AddFlashCardToStack()
    {
        string currentStack = ChooseStack();

        var (word, translation) = input.GetUserInput();

        using (var connection = db.GetConnection())
        {
            int currentStackID = db.GetStackId(currentStack);

            string sql = """
                        IF NOT EXISTS (SELECT 1 FROM Flashcards WHERE StackId = @StackId AND Word = @Word)
                            INSERT INTO Flashcards (StackId, Word, Translation) VALUES (@StackId, @Word, @Translation)
                        """;

            var card = new FlashCard
            {
                StackId = currentStackID,
                Word = word,
                Translation = translation
            };

            int rowsAffected = connection.Execute(sql, card);

            if (rowsAffected <= 0)
            {
                AnsiConsole.MarkupLine($"[red]'{word}' already exists in the {currentStack} stack.[/]");
            }
            else
            {
                AnsiConsole.MarkupLine($"[green]Flashcard added sucessfully to the {currentStack} stack[/]");
            }
        }

        UIHelper.PressAnyKey();
    }
    private void DeleteStack()
    {
        int stackId = UIHelper.PromptId("delete", "stack");

        bool confirmation = AnsiConsole.Confirm("[red]Are you sure you want to delete this stack? All Cards in the stack will be deleted too.[/]");

        if (!confirmation)
        {
            return;
        }

        using (var connection = db.GetConnection())
        {
            if (!db.StackExists(stackId))
            {
                AnsiConsole.MarkupLine($"[red]Stack {stackId} does not exist.[/].");
                UIHelper.PressAnyKey();
                return;
            }

            connection.Execute("DELETE FROM Stacks WHERE Id = @Id", new { Id = stackId });

            AnsiConsole.MarkupLine($"[green]Stack with Id {stackId} deleted successfully[/]");
        }

        UIHelper.PressAnyKey();


    }

    private void AddStack()
    {
        var stackName = AnsiConsole.Ask<string>("Please input the name of the stack to add: ");

        using (var connection = db.GetConnection())
        {
            string insertSql = """
                        IF NOT EXISTS (SELECT 1 FROM Stacks WHERE StackName = @StackName)
                            INSERT INTO Stacks (StackName) VALUES (@StackName)
                        """;

            var newStack = new Stacks
            {
                StackName = stackName.ToLower(),
            };

            int rowsAffected = connection.Execute(insertSql, newStack);

            if (rowsAffected <= 0)
            {
                AnsiConsole.MarkupLine($"[red]'{stackName}' already exists in the database[/]");
            }
            else
            {
                AnsiConsole.MarkupLine($"[green]Stack {stackName} added sucessfully to the database[/]");
            }
        }

        UIHelper.PressAnyKey();
    }

    private void EditStack()
    {
        var stackName = AnsiConsole.Ask<string>("Please input the name of the stack to update: ");

        using (var connection = db.GetConnection())
        {
            int stackId = db.GetStackId(stackName);

            if (!db.StackExists(stackId))
            {
                AnsiConsole.MarkupLine($"[red]Stack '{stackName}' does not exist.[/]");
                UIHelper.PressAnyKey();
                return;
            }

            var newStackName = AnsiConsole.Ask<string>("Please input the new name for the stack: ").ToLower();

            if (string.IsNullOrWhiteSpace(newStackName))
            {
                AnsiConsole.MarkupLine("[red]New stack name cannot be empty.[/]");
                UIHelper.PressAnyKey();
                return;
            }

            if (newStackName == stackName.ToLower())
            {
                AnsiConsole.MarkupLine("[yellow]The new name is the same as the current name. No changes made.[/]");
                UIHelper.PressAnyKey();
                return;
            }

            int existingId = db.GetStackId(newStackName);
            if (db.StackExists(existingId))
            {
                AnsiConsole.MarkupLine($"[red]A stack with the name '{newStackName}' already exists.[/]");
                UIHelper.PressAnyKey();
                return;
            }

            string updateSql = "UPDATE Stacks SET StackName = @StackName WHERE Id = @Id";

            connection.Execute(updateSql, new { StackName = newStackName, Id = stackId });

            AnsiConsole.MarkupLine($"[green]Stack '{stackName}' updated to '{newStackName}' successfully[/]");
        }

        UIHelper.PressAnyKey();
    }

    public void GetAllStacks()
    {
        using (var connection = db.GetConnection())
        {
            var table = new Table();
            table.Border(TableBorder.Rounded);

            table.AddColumn("[yellow]Name[/]");

            var stacks = connection.Query<StackResponseDto>("SELECT * FROM Stacks").ToList();

            if (stacks.Count == 0)
            {
                AnsiConsole.MarkupLine("[red]No rows found in the database[/]");
            }
            else
            {
                foreach (var stack in stacks)
                {
                    table.AddRow(
                        $"[purple]{stack.StackName}[/]".ToUpper()
                        );
                }

                AnsiConsole.Write(table);
            }
        }
    }
}
