using Flashcards.kilozdazolik.Data;
using Flashcards.kilozdazolik.Models;
using Flashcards.kilozdazolik.Dto;
using Spectre.Console;

namespace Flashcards.kilozdazolik.Controller;

public class FlashcardController
{
    private readonly FlashcardRepository _flashcardRepository = new();
    private readonly StackRepository _stackRepository = new();
    private readonly Helper _helper = new();

    public void DeleteFlashcard()
    {
        var stack = _helper.SelectStack(_stackRepository, "select");
        var flashCards = _flashcardRepository.GetCardsByStack(stack.StackId);
        
        var selectedFlashcard = _helper.SelectFlashcard(flashCards);
        if (_helper.ConfirmMessage("Delete", selectedFlashcard.Front))
        {
            _flashcardRepository.DeleteCard(selectedFlashcard);
        }
        else
        {
            AnsiConsole.MarkupLine("Deletion canceled.");
        }
        
        AnsiConsole.MarkupLine("[green]Press Any Key to Continue.[/]");
        Console.ReadKey();
    }
    
    public void EditFlashcard()
    {
        var stack = _helper.SelectStack(_stackRepository, "select");
        var flashCards = _flashcardRepository.GetCardsByStack(stack.StackId);
        
        var selectedFlashcard = _helper.SelectFlashcard(flashCards);
        
        var frontText = _helper.GetUserInputText("front text of the flashcard");
        var backText = _helper.GetUserInputText("back text of the flashcard");
        
        selectedFlashcard.Front = frontText;
        selectedFlashcard.Back = backText;
        
        try
        {
            _flashcardRepository.UpdateCard(selectedFlashcard);
            AnsiConsole.MarkupLine("[green]Flashcard successfully created![/]");
        }
        catch (InvalidOperationException ex)
        {
            AnsiConsole.MarkupLine($"[red]{ex.Message}[/]");
        }
        catch (Exception ex)
        {
            AnsiConsole.MarkupLine($"[red]Something went wrong while creating the Flashcard. ({ex.Message})[/]");
        }
    }
    
    public void CreateFlashcard()
    {
        var stack = _helper.SelectStack(_stackRepository, "select");
        
        AnsiConsole.Clear();
        
        var frontText = _helper.GetUserInputText("front text of the flashcard");
        var backText = _helper.GetUserInputText("back text of the flashcard");
        
        Flashcard flashcard = new()
        {
            StackId = stack.StackId,
            Front =  frontText,
            Back = backText,
        };
        
        try
        {
            _flashcardRepository.InsertCard(flashcard);
            AnsiConsole.MarkupLine("[green]Flashcard successfully updated![/]");
        }
        catch (InvalidOperationException ex)
        {
            AnsiConsole.MarkupLine($"[red]{ex.Message}[/]");
        }
        catch (Exception)
        {
            AnsiConsole.MarkupLine("[red]Something went wrong while updating the Flashcard.[/]");
        }
    }

    public void ViewFlashcards()
    {
        var stack = _helper.SelectStack(_stackRepository, "view flashcards");
        
        var flashCards = _flashcardRepository.GetCardsByStack(stack.StackId);
        
        List<FlashcardDto> flashCardList = new List<FlashcardDto>();
        int counter = 1;
        foreach (var elem in flashCards)
        {
            var flashCardDto = new FlashcardDto()
            {
                Id = counter,
                Front =  elem.Front,
                Back =  elem.Back
            };
            flashCardList.Add(flashCardDto);
            counter++;
        }
        
        if (flashCardList.Any())
        {
            var table = new Table();
            table.Border(TableBorder.Rounded);

            table.AddColumn("[yellow]ID[/]");
            table.AddColumn("[yellow]Front[/]");
            table.AddColumn("[yellow]Back[/]");

            foreach (var card in flashCardList)
            {
                table.AddRow(
                    $"[green]{card.Id.ToString()}[/]",
                    $"[blue]{card.Front}[/]",
                    $"[blue]{card.Back}[/]"
                );
            }

            AnsiConsole.Write(table);
        }
        else
        {
            AnsiConsole.MarkupLine("There are no cards available");
        }
        
    }
}