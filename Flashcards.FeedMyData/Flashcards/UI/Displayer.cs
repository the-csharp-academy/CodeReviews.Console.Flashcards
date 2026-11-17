using Spectre.Console;

namespace Flashcards;

internal class Displayer
{
    internal static void PrintTitle()
    {
        string path = Path.Combine(AppContext.BaseDirectory,
                                    @"UI\DancingFont.flf");
        var font = FigletFont.Load(path);
        Console.Clear();
        var figlet = new FigletText("Flashcards").Color(Color.SeaGreen1);
        AnsiConsole.Write(figlet);
        AnsiConsole.WriteLine();
    }

    internal static void Loading(string title, Action<ProgressTask> work)
    {
        AnsiConsole.MarkupLine($"[bold yellow]{title}[/]");
        AnsiConsole.Progress()
            .Columns(
                new TaskDescriptionColumn(),
                new ProgressBarColumn(),
                new PercentageColumn(),
                new ElapsedTimeColumn())
            .Start(context =>
            {
                var task = context.AddTask("Will be overwritten", maxValue: 3);
                work(task);
            });
    }

    internal static void GenericError(string message)
    {
        AnsiConsole.MarkupLine($"[Orange1]{message}[/]");
        GoBackPressKey();
    }

    internal static void GoBackPressKey()
    {
        AnsiConsole.MarkupLine($"[grey]Press any key to go back.[/]");
        Console.ReadKey();
    }

    internal static void CrudSuccess(string thing, string adjective)
    {
        AnsiConsole.MarkupLine($"[green]The {thing} has been {adjective}.[/]");
        AnsiConsole.WriteLine();
    }

    internal static void CrudDiscard(string thing, string adjective)
    {
        AnsiConsole.MarkupLine($"[Orange1]The {thing} has been {adjective}.[/]");
        AnsiConsole.WriteLine();
    }

    internal static void CleanConsole()
    {
        Console.Clear();
        PrintTitle();
    }

    internal static void PrintTable(List<FlaschcardDto> flashcards)
    {
        Table table = new Table()
        .RoundedBorder()
        .AddColumn("N")
        .AddColumn("Recto")
        .AddColumn("Verso");

        foreach (var entry in flashcards)
        {
            string[] rowData = { entry.DisplayId.ToString(), entry.Recto, entry.Verso };
            TableExtensions.AddRow(table, rowData);
        }
        AnsiConsole.Write(table);
    }

    internal static void PrintTable(List<StudyDto> studies)
    {
        Table table = new Table()
        .RoundedBorder()
        .AddColumn("Date")
        .AddColumn("Stack")
        .AddColumn("Score")
        .AddColumn("%");

        foreach (var entry in studies)
        {
            string percentage;

            if (entry.Percentage < 50)
                percentage = $"[red]{entry.Percentage}%[/]";
            else if (entry.Percentage > 85)
                percentage = $"[green]{entry.Percentage}%[/]";
            else
                percentage = entry.Percentage + "%";

            string[] rowData = {
                DateOnly.FromDateTime(entry.Date).ToString(),
                entry.Name,
                entry.Score.ToString() + "/" + entry.ScoreMax.ToString(),
                percentage };
            TableExtensions.AddRow(table, rowData);
        }
        AnsiConsole.Write(table);
    }

    internal static void PrintPanel(string content, Color color, string title)
    {
        AnsiConsole.Write(
            new Panel($"[{color}]{content}[/]")
            { Width = 30 }
                .Header(title, Justify.Center)
                .RoundedBorder()
                .BorderColor(Color.MediumPurple)
                .Padding(2, 2));
    }

    internal static string GoodAnswer()
    {
        Random rand = new();
        string[] goodAnswer = {
            "Good!",
            "Well done!",
            "You got it!",
            "Right!",
            "Exactly.",
            "On point.",
            "Noice.",
            "Yes!",
            "Yepi Yep!",
            "Goosebumps.",
            "True champ."
        };

        return goodAnswer[rand.Next(0, goodAnswer.Length)];
    }

    internal static string WrongAnswer()
    {
        Random rand = new();
        string[] wrongAnswer = {
            "Almost?",
            "Nope.",
            "Nein.",
            "Nononono...",
            "You'll got it next time.",
            "Wrong.",
            "Plain wrong",
            "Oups.",
            "Maybe a typo?",
            "It was a hard one.",
            "Mistakes happen",
            "Mistakes are part of life.",
        };

        return wrongAnswer[rand.Next(0, wrongAnswer.Length)];
    }

    internal static void MaxScoreCongrats()
    {
        Random rand = new();
        string[] veryGoodScore = {
            "Awesome!",
            "Fantastic",
            "Amazing"
        };
        var font = FigletFont.Load(Path.Combine(
                            AppContext.BaseDirectory,
                            @"UI\DancingFont.flf"));

        var figlet = new FigletText(font, veryGoodScore[rand.Next(0, veryGoodScore.Length)])
                        .Color(Color.Green);
        AnsiConsole.Write(figlet);
    }
}