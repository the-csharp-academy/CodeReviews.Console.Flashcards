using Flashcards.DTOs;
using Flashcards.Services;
using Spectre.Console;

namespace Flashcards.UI
{
    internal class ReportsMenu
    {
        private readonly ReportingService _reportingService;
        private readonly StackService _stackService;
        private readonly StudySessionService _studySessionService;
        public ReportsMenu(ReportingService reportingService, StackService stackService, StudySessionService studySessionService)
        {
            _reportingService = reportingService;
            _stackService = stackService;
            _studySessionService = studySessionService;
        }

        internal async Task Show()
        {
            bool show = true;

            while (show)
            {
                AnsiConsole.Clear();

                var stacks = await _stackService.GetAllStacksDTOAsync(null);

                var choice = AnsiConsole.Prompt(new SelectionPrompt<ReportMenuOptions>()
                    .Title("What would you like to do?")
                    .UseConverter(x => x switch
                    {
                        ReportMenuOptions.All => "Show all history",
                        ReportMenuOptions.Stack => "Show filtered history by Stack",
                        ReportMenuOptions.Monthly => "Show Monthly study session count by year",
                        ReportMenuOptions.MonthlyAvg => "Show Monthly study session avg by year",
                        ReportMenuOptions.back => "back"
                    })
                    .AddChoices(Enum.GetValues<ReportMenuOptions>()));

                switch (choice)
                {
                    case ReportMenuOptions.All:
                        var studySessions = await _studySessionService.CreateStudySessionDTOAsync(null);
                        var table = TableBuilder.ShowStudySessionsTable(studySessions, stacks);
                        AnsiConsole.Write(table);
                        AnsiConsole.MarkupLine("[bold]Press any key to continue...[/]");
                        Console.ReadKey();
                        break;
                    case ReportMenuOptions.Stack:
                        await ShowStudySessionHistoryByStack();
                        break;
                    case ReportMenuOptions.Monthly:
                        await ShowMonthlyStudySessionReportAsync();
                        break;
                    case ReportMenuOptions.MonthlyAvg:
                        await ShowMonthlyStudySessionAvgReportAsync();
                        break;
                    case ReportMenuOptions.back:
                        show = false;
                        break;
                }
            }
        }

        internal async Task ShowStudySessionHistoryByStack()
        {
            AnsiConsole.Clear();
            var stacks = await _stackService.GetAllStacksDTOAsync(null);

            var stack = AnsiConsole.Prompt(new SelectionPrompt<StackDTO>()
                .Title("Please select the stack you want to view")
                .UseConverter(x =>
                {
                    return $"{x.Name}";
                })
                .AddChoices(stacks));

            var studySessions = await _studySessionService.CreateStudySessionDTOAsync(stack);
            var table = TableBuilder.ShowStudySessionsTable(studySessions, stacks);

            AnsiConsole.Write(table);
            AnsiConsole.MarkupLine("[bold]Press any key to continue...[/]");
            Console.ReadKey();
        }

        internal async Task ShowMonthlyStudySessionReportAsync()
        {
            AnsiConsole.Clear();

            var year = AnsiConsole.Prompt(new TextPrompt<Int32>("Please enter the year you want to view")
                .Validate(year =>
                {
                    if (year < 2000 || year > DateTime.Now.Year)
                    {
                        return ValidationResult.Error("[red]Please enter a valid date between the year 2000 and the current year[/]");
                    }

                    return ValidationResult.Success();
                }));

            var list = await _reportingService.GetMonthlyStudySessionsAsync(year);
            var table = TableBuilder.BuildMonthlyTable(list);

            AnsiConsole.Clear();

            AnsiConsole.MarkupLine($"[bold cyan]Showing results for year: {year}[/] \n\n");
            AnsiConsole.Write(table);
            AnsiConsole.MarkupLine("[bold]Press any key to continue[/]");
            Console.ReadKey();
        }

        internal async Task ShowMonthlyStudySessionAvgReportAsync()
        {
            AnsiConsole.Clear();

            var year = AnsiConsole.Prompt(new TextPrompt<Int32>("Please enter the year you want to view")
                .Validate(year =>
                {
                    if (year < 2000 || year > DateTime.Now.Year)
                    {
                        return ValidationResult.Error("[red]Please enter a valid date between the year 2000 and the current year[/]");
                    }

                    return ValidationResult.Success();
                }));

            var list = await _reportingService.GetMonthlyStudyAvgAsync(year);
            var table = TableBuilder.BuildMonthlyTable(list);

            AnsiConsole.Clear();

            AnsiConsole.MarkupLine($"[bold cyan]Showing results for year: {year} \n\n[/]");
            AnsiConsole.Write(table);
            AnsiConsole.MarkupLine("[bold]Press any key to continue[/]");
            Console.ReadKey();
        }
    }

    enum ReportMenuOptions
    {
        All,
        Stack,
        Monthly,
        MonthlyAvg,
        back
    }
}
