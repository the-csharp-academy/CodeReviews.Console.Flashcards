using FlashcardsApp.Repositories;
using FlashcardsApp.Utilities;
using System;
using System.Text;
using System.Text.RegularExpressions;

namespace FlashcardsApp
{
    public class StudyMenu
    {
        private readonly SessionRepository _sessionRepo;
        private const int Width = 15;
        private readonly string border = new string('-', Width-2) + "+";
        private readonly string line = new string('-', Width);

        public StudyMenu(SessionRepository sessionRepo)
        {
            _sessionRepo = sessionRepo;
        }

        public void ShowStudyMenu()
        {
            Console.Clear();
            int year;
            Console.WriteLine(line);
            Console.Write("Input a year in format YYYY\n");
            Console.WriteLine(line);
            string? inp = Console.ReadLine();
            while (!int.TryParse(inp, out year))
            {
                Console.Write("\nPlease enter a valid number.\nTry again: ");
                inp = Console.ReadLine();
            }

            var data = _sessionRepo.GetStatByMonth(year);
            var months = new List<string>{"January", "February", "March", "April", "May", "June", "July", "August", "September", "October", "November", "December"};

            int nameWidth = 10;
            // Determine the StackName column width based on the longest stack name
            foreach (var stackName in data.Keys)
            {
                if (stackName.Length + 2 > nameWidth)
                    nameWidth = stackName.Length + 2;
            }

            string cellBorder = new string('-', Width - 2) + "+";
            string nameBorder = new string('-', nameWidth) + "+";

            Console.WriteLine("\n" + $" Average per month for: {year} ".PadCenter(nameWidth + 1 + Width * 12, '-'));

            // Header row
            Console.Write("+" + nameBorder + string.Concat(Enumerable.Repeat(cellBorder, 12)));
            Console.Write("\n| " + "StackName".PadRight(nameWidth - 1) + "| ");
            foreach (var month in months)
            {
                Console.Write(month.PadCenter(Width - 4) + " | ");
            }

            // Data rows
            Console.Write("\n+" + nameBorder + string.Concat(Enumerable.Repeat(cellBorder, 12)));
            foreach (var entry in data)
            {
                Console.Write("\n| " + entry.Key.PadRight(nameWidth - 1) + "| ");
                foreach (var count in entry.Value)
                {
                    Console.Write(count.ToString().PadCenter(Width - 4) + " | ");
                }
                Console.Write("\n+" + nameBorder + string.Concat(Enumerable.Repeat(cellBorder, 12)));
            }

            if (data.Count == 0)
            {
                Console.Write("\n| " + "No data".PadRight(nameWidth - 1) + "| ");
                for (int i = 0; i < 12; i++)
                {
                    Console.Write("0".PadCenter(Width - 4) + " | ");
                }
                Console.Write("\n+" + nameBorder + string.Concat(Enumerable.Repeat(cellBorder, 12)));
            }

            Console.WriteLine("\n\nPress any key to continue");
            Console.ReadKey();
        }
    }
}
