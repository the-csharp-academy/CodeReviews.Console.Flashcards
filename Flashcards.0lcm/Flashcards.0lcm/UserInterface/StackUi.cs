using Flashcards._0lcm.DTOs;
using Flashcards._0lcm.Interfaces;
using Flashcards._0lcm.Models;
using Flashcards._0lcm.Services;
using Spectre.Console;
using Spectre.Console.Rendering;

namespace Flashcards._0lcm.UserInterface;

internal class StackUi(IStackService stackService)
{
    private const string ReturnOption = "* Back";
    private const string CreateOption = "* Create a new stack";
    private readonly IStackService _stackService = stackService;

    //------- Entry Point Menu -------
    internal void StackArea()
    {
        ViewStacks();
    }

    //------- Viewing Methods -------
    private void ViewStacks()
    {
        var hasLoaded = false;
        while (true)
        {
            Console.Clear();

            DisplayHelper.DisplayInfo("Scroll using <Up> and <Down>, press <Enter> to select.");
            DisplayHelper.DisplayInfo(
                "Press 'Back' Or 'Create new stack' to take an action. Or select any existing stack.");
            if (!hasLoaded)
            {
                DisplayHelper.DisplaySpinner("Loading stacks..", 2000);
                hasLoaded = true;
            }

            var stackMap = UtilityService.BuildStackMap(_stackService, ReturnOption, CreateOption);

            var choice = DisplayHelper.DisplayPrompt(
                stackMap.Keys.ToList(),
                "\n Options:");

            if (choice == ReturnOption) return;
            if (choice == CreateOption)
            {
                CreateStack();
                return;
            }

            LoadStack(stackMap[choice]!);
        }
    }

    private void LoadStack(StackDto dto)
    {
        Console.Clear();
        var properties = new List<IRenderable>
        {
            new Markup($"[{DisplayHelper.White}]ID: {dto.DisplayId}[/]"),
            new Markup($"[{DisplayHelper.White}]Name: {dto.Name}[/]"),
            new Markup($"[{DisplayHelper.White}]Flashcard Count: {dto.FlashcardCount}[/]")
        };
        DisplayHelper.DisplayRows(properties);
        DisplayHelper.DisplayMessage("\n");

        var choice = DisplayHelper.DisplayMenu<Enums.StackAreaOption>();

        switch (choice)
        {
            case Enums.StackAreaOption.ChangeName:
                ChangeStackName(dto);
                break;
            case Enums.StackAreaOption.Delete:
                DeleteStack(dto);
                break;
            case Enums.StackAreaOption.Back:
                return;
        }
    }

    //------- CRUD Operations -------
    private void CreateStack()
    {
        while (true)
        {
            Console.Clear();

            var name = DisplayHelper.DisplayQuestion("Please enter a name for the stack:");

            try
            {
                _stackService.CreateStack(name);
                DisplayHelper.DisplaySuccess("Succesfully changed stack name. Press enter to continue.");
                Console.ReadLine();
                return;
            }
            catch (ArgumentNullException ex)
            {
                DisplayHelper.DisplayWarning(ex.Message ??
                                             "Invalid name. Please check that the name is not null or empty.");
                DisplayHelper.DisplayInfo("\nPress <Enter> to continue.");
                Console.ReadLine();
            }
            catch (ArgumentException ex)
            {
                DisplayHelper.DisplayWarning(ex.Message ??
                                             "Invalid name. Please check that the name is not already taken.");
                DisplayHelper.DisplayInfo("\nPress <Enter> to continue.");
                Console.ReadLine();
            }
        }
    }

    private void ChangeStackName(StackDto dto)
    {
        while (true)
        {
            var name = DisplayHelper.DisplayQuestion("Please enter a name for the stack:");

            try
            {
                _stackService.UpdateStack(dto, name);
                DisplayHelper.DisplaySuccess("Successfully changed stack name. Press enter to continue.");
                Console.ReadLine();
                return;
            }
            catch (ArgumentNullException ex)
            {
                DisplayHelper.DisplayWarning(ex.Message ??
                                             "Invalid name. Please check that the name is not null or empty.");
                DisplayHelper.DisplayInfo("\nPress <Enter> to continue.");
                Console.ReadLine();
            }
            catch (ArgumentException ex)
            {
                DisplayHelper.DisplayWarning(ex.Message ??
                                             "Invalid name. Please check that the name is not already taken.");
                DisplayHelper.DisplayInfo("\nPress <Enter> to continue.");
                Console.ReadLine();
            }
        }
    }

    private void DeleteStack(StackDto dto)
    {
        if (AnsiConsole.Confirm($@"[{DisplayHelper.Yellow}]Are you sure you want to delete this stack?
This will also delete all flashcards and study sessions associated with this stack.[/]"))
        {
            _stackService.DeleteStack(dto);
            DisplayHelper.DisplaySuccess("Successfully deleted stack and all dependents. Press enter to continue.");
            Console.ReadLine();
        }
        else
        {
            DisplayHelper.DisplayWarning("Cancelled deletion, press enter to continue.");
            Console.ReadLine();
        }
    }
}