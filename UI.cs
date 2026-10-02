using Flashcards.DTOs;
using Spectre.Console;

namespace Flashcards
{
    public static class UI
    {
        public static string AskText(string prompt, int maxLength)
        {
            return AnsiConsole.Prompt(new TextPrompt<string>(prompt)
                .Validate(text =>
                {
                    if (string.IsNullOrWhiteSpace(text))
                        return ValidationResult.Error("Text cannot be empty or whitespace.");
                    if (text.Trim().Length > maxLength)
                        return ValidationResult.Error($"Text must contain at most {maxLength} characters.");
                    return ValidationResult.Success();
                })).Trim();
        }

        public static int AskNumber(string prompt, int min, int max)
        {
            return AnsiConsole.Prompt(new TextPrompt<int>(prompt)
                .ValidationErrorMessage("Enter a whole number.")
                .Validate(value => value >= min && value <= max
                    ? ValidationResult.Success()
                    : ValidationResult.Error($"Enter a number between {min} and {max}.")));
        }

        public static void PrintMessage(string message)
        {
            AnsiConsole.WriteLine(message);
        }

        public static string GetActions(List<string> actions)
        {
            var str = AnsiConsole
                .Prompt(new SelectionPrompt<string>()
                    .Title("Choose action")
                    .UseConverter(action => Markup.Escape(action))
                    .AddChoices(actions));
            return str;
        }

        public static void PrintStackTable(List<CardStackDTO> stacks)
        {
            var table = new Table()
                .AddColumn("Name");
            foreach (var stack in stacks)
                table.AddRow(Markup.Escape(stack.Name));
            AnsiConsole.Write(table);
        }

        public static void PrintFlashcardTable(List<GetFlashcardDTO> flashcards)
        {
            var number = 1;
            var table = new Table()
                .AddColumn("Id")
                .AddColumn("Front")
                .AddColumn("Back");
            foreach (var flashcard in flashcards)
            {
                table.AddRow(number.ToString(), Markup.Escape(flashcard.Front), Markup.Escape(flashcard.Back));
                number++;
            }
            AnsiConsole.Write(table);
        }

        public static List<int> PrintStackChoices(List<CardStackDTO> stacks)
        {
            if (stacks.Count == 0)
            {
                AnsiConsole.MarkupLine("No stacks yet;");
                return null;
            }

            var multiPrompt = new MultiSelectionPrompt<string>()
                .Title("Choose stack")
                .UseConverter(name => Markup.Escape(name))
                .NotRequired()
                .InstructionsText("[grey](Press [blue]<space>[/] to toggle, [green]<enter>[/] to confirm)[/]");

            foreach (var stack in stacks)
            {
                multiPrompt.AddChoice(stack.Name);
            }

            var choices = AnsiConsole.Prompt(multiPrompt);
            return stacks
                .Where(s => choices.Contains(s.Name))
                .Select(s => s.Id)
                .ToList();
        }

        public static List<int> PrintFlashcardChoices(List<GetFlashcardDTO> flashcards)
        {
            if (flashcards.Count == 0)
            {
                AnsiConsole.MarkupLine("No flashcards yet;");
                return new List<int>();
            }

            var multiPrompt = new MultiSelectionPrompt<GetFlashcardDTO>()
                .Title("Choose flashcards")
                .UseConverter(f => Markup.Escape(f.Front + " - " + f.Back))
                .NotRequired()
                .InstructionsText("[grey](Press [blue]<space>[/] to toggle, [green]<enter>[/] to confirm)[/]");

            foreach (var flashcard in flashcards)
            {
                multiPrompt.AddChoice(flashcard);
            }

            var choices = AnsiConsole.Prompt(multiPrompt);
            return choices
                .Select(s => s.Id)
                .ToList();
        }
    }
}
