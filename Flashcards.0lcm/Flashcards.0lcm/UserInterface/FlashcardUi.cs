using Flashcards._0lcm.DTOs;
using Flashcards._0lcm.Interfaces;
using Flashcards._0lcm.Models;
using Flashcards._0lcm.Services;
using Spectre.Console;
using Spectre.Console.Rendering;

namespace Flashcards._0lcm.UserInterface;

internal class FlashcardUi(IFlashcardService flashcardService, IStackService stackService)
{
    private const string ReturnOption = "* Back";
    private const string CreateOption = "* Create a new flashcard";
    private readonly IFlashcardService _flashcardService = flashcardService;
    private readonly IStackService _stackService = stackService;

    //------- Entry Point Menu -------
    internal void FlashcardArea()
    {
        ViewFlashcards();
    }

    //------- Viewing Methods -------
    private void ViewFlashcards()
    {
        var hasLoaded = false;
        while (true)
        {
            Console.Clear();

            DisplayHelper.DisplayInfo("Scroll using <Up> and <Down>, press <Enter> to select.");
            DisplayHelper.DisplayInfo(
                "Press 'Back' or 'Create flashcard' to take an action, or select any existing option.");
            if (!hasLoaded)
            {
                DisplayHelper.DisplaySpinner("Loading flashcards..", 2000);
                hasLoaded = true;
            }

            var stackMap = UtilityService.BuildStackMap(_stackService, ReturnOption);

            var stackChoice = DisplayHelper.DisplayPrompt(
                stackMap.Keys.ToList(),
                "\nSelect a stack to view flashcards:");

            if (stackChoice == ReturnOption) return;

            var stackDto = stackMap[stackChoice];
            var flashcardMap = UtilityService.BuildFlashcardMap(
                _flashcardService,
                stackDto!,
                ReturnOption,
                CreateOption);

            var flashcardChoice = DisplayHelper.DisplayPrompt(
                flashcardMap.Keys.ToList(),
                "\n Options:");

            switch (flashcardChoice)
            {
                case ReturnOption:
                    return;
                case CreateOption:
                    CreateFlashcard(stackDto!);
                    continue;
            }

            LoadFlashcard(flashcardMap[flashcardChoice]!);
        }
    }

    private void LoadFlashcard(FlashcardDto flashcardDto)
    {
        Console.Clear();

        var properties = new List<IRenderable>
        {
            new Markup($"[{DisplayHelper.White}]ID: {flashcardDto.DisplayId}[/]"),
            new Markup($"[{DisplayHelper.White}]Name: {flashcardDto.Name}[/]"),
            new Markup($"[{DisplayHelper.White}]Value: {flashcardDto.Value}[/]"),
            new Markup($"[{DisplayHelper.White}]Belonging to Stack: {flashcardDto.StackName}[/]")
        };
        DisplayHelper.DisplayRows(properties);
        DisplayHelper.DisplayMessage("\n");

        var choice = DisplayHelper.DisplayMenu<Enums.FlashcardAreaOption>();

        switch (choice)
        {
            case Enums.FlashcardAreaOption.UpdateNameAndValue:
                UpdateFlashcardNameAndValue(flashcardDto);
                break;
            case Enums.FlashcardAreaOption.Delete:
                DeleteFlashcard(flashcardDto);
                break;
            case Enums.FlashcardAreaOption.Back:
                return;
        }
    }

    //------- CRUD Operations -------
    private void CreateFlashcard(StackDto dto)
    {
        Console.Clear();

        string? name = null;
        string? value = null;

        while (true)
        {
            name = DisplayHelper.DisplayQuestion("Please enter a name for your flashcard:");
            if (string.IsNullOrEmpty(name))
            {
                DisplayHelper.DisplayWarning("Your name is either null or empty, please fix this.");
                continue;
            }

            value = DisplayHelper.DisplayQuestion("Please enter a value for your flashcard:");
            if (string.IsNullOrEmpty(value))
            {
                DisplayHelper.DisplayWarning("Your value is either null or empty, please fix this.");
                continue;
            }

            break;
        }

        _flashcardService.CreateFlashcard(name, value, dto.StackId);
        DisplayHelper.DisplaySuccess("Succesfully created flashcard! Press enter to continue.");
        Console.ReadLine();
    }

    private void UpdateFlashcardNameAndValue(FlashcardDto flashcardDto)
    {
        while (true)
        {
            var name = DisplayHelper.DisplayQuestion("Please enter a name for your flashcard:");
            var value = DisplayHelper.DisplayQuestion("Please enter a value for your flashcard:");

            try
            {
                _flashcardService.UpdateFlashcard(flashcardDto.CardId, name, value);
                DisplayHelper.DisplaySuccess("Succesfully updated flashcard. Press enter to continue.");
                Console.ReadLine();
                return;
            }
            catch (ArgumentNullException ex)
            {
                DisplayHelper.DisplayWarning(ex.Message ??
                                             "One or all of your inputs are null or empty, Please fix this.");
                DisplayHelper.DisplayInfo("Press <Enter> to continue.");
                Console.ReadLine();
            }
        }
    }

    private void DeleteFlashcard(FlashcardDto flashcardDto)
    {
        if (AnsiConsole.Confirm("Are you sure you want to permanently delete this flashcard?"))
            _flashcardService.DeleteFlashcard(flashcardDto.CardId);

        DisplayHelper.DisplaySuccess("Succesfully deleted flashcard. Press enter to continue.");
        Console.ReadLine();
    }
}