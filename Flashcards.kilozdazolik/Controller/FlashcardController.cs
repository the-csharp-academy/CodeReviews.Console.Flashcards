using Flashcards.kilozdazolik.Data;
using Flashcards.kilozdazolik.Models;
using Flashcards.kilozdazolik.Dto;
using Spectre.Console;

namespace Flashcards.kilozdazolik.Controller;

public class FlashcardController
{
    private FlashcardRepository _flashcardRepository = new();
    private StackRepository _stackRepository = new();
    private Helper _helper = new();

    public void DeleteFlashcard()
    {
        var stack = _helper.SelectStack(_stackRepository, "select");
        var flashCards = _flashcardRepository.GetCardsByStack(stack.StackId);
    
        // Let user select directly from Flashcard objects
        var selectedFlashcard = AnsiConsole.Prompt(
            new SelectionPrompt<Flashcard>()
                .Title("Select a [cyan]FLASHCARD[/]:")
                .UseConverter(f => $"{f.Front} - {f.Back}")
                .AddChoices(flashCards)
        );
        
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
    
        // Let user select directly from Flashcard objects
        var selectedFlashcard = AnsiConsole.Prompt(
            new SelectionPrompt<Flashcard>()
                .Title("Select a [cyan]FLASHCARD[/]:")
                .UseConverter(f => $"{f.Front} - {f.Back}")
                .AddChoices(flashCards)
        );
    
        // Get new data
        var frontText = _helper.GetUserInputText("front text of the flashcard");
        var backText = _helper.GetUserInputText("back text of the flashcard");
    
        // Update the selected flashcard
        selectedFlashcard.Front = frontText;
        selectedFlashcard.Back = backText;
    
        // Update in database
        try
        {
            _flashcardRepository.UpdateCard(selectedFlashcard);
            AnsiConsole.MarkupLine("[green]Stack successfully created![/]");
        }
        catch (InvalidOperationException ex)
        {
            AnsiConsole.MarkupLine($"[red]{ex.Message}[/]");
        }
        catch (Exception ex)
        {
            AnsiConsole.MarkupLine("[red]Something went wrong while creating the stack.[/]");
        }
    }
    
    public void CreateFlashcard()
    {
        // The user must select a stack first
        List<Stack> allStacks = _stackRepository.GetAllStacks();

        if (allStacks.Count == 0)
        {
            AnsiConsole.MarkupLine("[red]No stacks are available. Create a stack first.[/]");
            Console.ReadKey();
            return;
        }
        
        var getStackId = AnsiConsole.Prompt(
            new SelectionPrompt<Stack>()
                .Title("Select a [cyan]STACK[/]:")
                .UseConverter(s => $"{s.Name}")
                .AddChoices(allStacks)
        );
        
        AnsiConsole.Clear();
        
        // after we got the stackId we are going to ask for the front and back text
        var frontText = _helper.GetUserInputText("front text of the flashcard");
        var backText = _helper.GetUserInputText("back text of the flashcard");

        // create a new flashcard object and assign the variables
        Flashcard flashcard = new()
        {
            StackId = getStackId.StackId,
            Front =  frontText,
            Back = backText,
        };

        // pass it to the db
        try
        {
            _flashcardRepository.InsertCard(flashcard);
            AnsiConsole.MarkupLine("[green]Stack successfully updated![/]");
        }
        catch (InvalidOperationException ex)
        {
            AnsiConsole.MarkupLine($"[red]{ex.Message}[/]");
        }
        catch (Exception)
        {
            AnsiConsole.MarkupLine("[red]Something went wrong while updating the stack.[/]");
        }
    }

    public void ViewFlashcards()
    {
        // select a stack to view the flashcards in
        var stack = _helper.SelectStack(_stackRepository, "view flashcards");
        if (stack == null) return;
        
        var flashCards = _flashcardRepository.GetCardsByStack(stack.StackId);
        
        // iterate and map through the flashcard list
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

        // dispaly dto
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