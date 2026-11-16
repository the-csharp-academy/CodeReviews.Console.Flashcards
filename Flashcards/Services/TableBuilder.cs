using Flashcards.DTOs;
using Flashcards.Interfaces;
using Spectre.Console;
using System.Globalization;

namespace Flashcards.Services
{
    public static class TableBuilder
    {
        public static Table ShowStackTable(List<StackDTO> stacks)
        {
            var table = new Table()
                .AddColumn("Id")
                .AddColumn("Name");

            foreach (var s in stacks)
            {
                table.AddRow(s.Number.ToString(), s.Name);
            }

            return table;
        }

        public static Table ShowFlashcardsTable(List<FlashcardDTO> flashcards, List<StackDTO> stacks)
        {
            var table = new Table()
                .AddColumns(["Id", "Front", "Back", "Stack"]);

            foreach (var f in flashcards)
            {
                var stack = stacks.Where(s => s.Id == f.StackId).Single();

                table.AddRow(f.Number.ToString(), f.Front, f.Back, stack.Name);
            }

            return table;
        }

        public static Table ShowStudySessionsTable(List<StudySessionDTO> studySessions, List<StackDTO> stacks)
        {
            var table = new Table()
                .AddColumns(["No.", "Stack", "Score", "StartTime", "Duration", "Percentage"]);

            foreach (var s in studySessions)
            {
                var stack = stacks.Where(l => l.Id == s.StackId).Single();

                table.AddRow(s.Number.ToString(), stack.Name, $"{s.Score}/{s.TotalCards}", s.StartTime.ToString(), s.Duration.ToString(), s.Percentage.ToString());
            }

            return table;
        }

        public static Table BuildMonthlyTable(IEnumerable<IMonthlyReport> list)
        {
            var table = new Table()
                .Expand()
                .AddColumn("StackId")
                .AddColumn("Stack Name", col => col.Width(30));

            for (int i = 1; i < 13; i++)
            {
                table.AddColumn(CultureInfo.InvariantCulture.DateTimeFormat.GetMonthName(i).Substring(0, 3));
            }

            foreach (var s in list)
            {
                var row = new List<string>
                {
                   s.StackId.ToString(),
                   s.StackName
                };

                row.AddRange(s.MonthlyValues.Select(v => v.ToString()));

                table.AddRow(row.ToArray());
            }

            return table;

        }

    }
}
