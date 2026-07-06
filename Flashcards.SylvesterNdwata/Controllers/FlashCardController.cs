using Dapper;
using Microsoft.Data.SqlClient;
using silvermax.FlashCards.Dtos;
using silvermax.FlashCards.Models;
using Spectre.Console;
using static silvermax.FlashCards.Enums;

namespace silvermax.FlashCards.Controllers;

internal class FlashCardController
{
    private readonly DatabaseHelper db = new();
    StacksController controller = new();
    UserInput input = new();
    public void Start()
    {
        var choice = AnsiConsole.Prompt(
        new SelectionPrompt<ManageFlashCard>()
        .Title("Choose an action to do on flashcards")
        .UseConverter(e => Enums.ToDisplayName(e))
        .AddChoices(Enum.GetValues<ManageFlashCard>()));

        switch (choice)
        {
            case ManageFlashCard.AddFlashcard:
                AddFlashCard();
                break;

            case ManageFlashCard.EditFlashcard:
                EditFlashCard();
                break;

            case ManageFlashCard.DeleteFlashcard:
                DeleteFlashCard();
                break;

            case ManageFlashCard.ViewAllFlashcards:
                ViewAllFlashCards();
                break;

            case ManageFlashCard.Back:
                return;
        }
    }

    private void ViewAllFlashCards()
    {
        using (var connection = db.GetConnection())
        {

            var cards = connection.Query<FlashCardResponseDto>("SELECT * FROM Flashcards").ToList();

            AnsiConsole.Write(UIHelper.BuildFlashCardTable(cards));
        }

        UIHelper.PressAnyKey();
    }

    private void DeleteFlashCard()
    {
        var word = AnsiConsole.Ask<string>("Please enter the word of the FlashCard you want to delete: ");

        if (!db.FlashCardExists(word))
        {
            AnsiConsole.MarkupLine($"[red]FlashCard with word {word} does not exist.[/]");
            UIHelper.PressAnyKey();
            return;
        }

        using (var connection = db.GetConnection())
        {
            connection.Execute("DELETE FROM Flashcards WHERE Word = @Word", new { Word = word });

            AnsiConsole.MarkupLine($"[green]Flashcard with word {word} deleted successfully[/]");
        }

        UIHelper.PressAnyKey();
    }

    private void EditFlashCard()
    {
        var wordToChange = AnsiConsole.Ask<string>("Please enter the word of the FlashCard you want to edit: ");
        var (word, translation) = input.GetUserInput();

        if (!db.FlashCardExists(wordToChange))
        {
            AnsiConsole.MarkupLine($"[red]FlashCard with word {wordToChange} does not exist.[/]");
            UIHelper.PressAnyKey();
            return;
        }

        using (var connection = db.GetConnection())
        {

            string updatesql = "UPDATE Flashcards SET Word = @NewWord, Translation = @Translation WHERE Word = @Word";

            var cardToUpdate = new 
            {
                NewWord = word,
                Word = wordToChange,
                Translation = translation,
            };

            connection.Execute(updatesql, cardToUpdate);

            AnsiConsole.MarkupLine($"[green]FlashCard with word {wordToChange} updated successfully[/]");
        }

        UIHelper.PressAnyKey();
    }

    private void AddFlashCard()
    {
        var (word, translation) = input.GetUserInput();
        controller.GetAllStacks();
        var stackName = AnsiConsole.Ask<string>("Please enter the Id of the Stack that this flashcard will be added to: ");

        using (var connection = db.GetConnection())
        {
            var stackId = db.GetStackId(stackName);

            if (!db.StackExists(stackId))
            {
                AnsiConsole.MarkupLine($"[red]Stack {stackName} does not exist.[/].");
                UIHelper.PressAnyKey();
                return;
            }

            string insertSql = """
                        IF NOT EXISTS (SELECT 1 FROM Flashcards WHERE StackId = @StackId AND Word = @Word)
                            INSERT INTO Flashcards (StackId, Word, Translation) VALUES (@StackId, @Word, @Translation)
                        """;

            var entry = new FlashCard
            {
                StackId = stackId,
                Word = word,
                Translation = translation
            };

            int rowsAffected = connection.Execute(insertSql, entry);

            if (rowsAffected <= 0)
            {
                AnsiConsole.MarkupLine($"[red]'{word}' already exists in the database[/]");
            }
            else
            {
                AnsiConsole.MarkupLine($"[green]Flashcard added sucessfully to the database[/]");
            }
        }

        UIHelper.PressAnyKey();
    }
}
