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
            Console.Write("Input a year in format YYYY:\t");
            string? inp = Console.ReadLine();
            while (!int.TryParse(inp, out year))
            {
                Console.Write("\nPlease enter a valid number.\nTry again: ");
                inp = Console.ReadLine();
            }

            var list = _sessionRepo.GetStatByMonth(year);
            var months = new List<string>{"January", "February", "March", "April", "May", "June", "July", "August", "September", "October", "November", "December"};

            Console.WriteLine("\n" +$" AVERAGE PER MONTH FOR: {year} ".PadCenter(Width*11, '='));
            Console.WriteLine("\n+" + string.Concat(Enumerable.Repeat(border, 12)));
            Console.Write("| ");
            foreach(var month in months)
            {
                Console.Write(month.PadCenter(Width - 4) + " | ");
            }
            Console.WriteLine("\n+" + string.Concat(Enumerable.Repeat(border, 12)));
            Console.Write("| ");
            foreach (var num in list)
            {
                Console.Write(num.ToString().PadCenter(Width - 4) + " | ");
            }
            Console.WriteLine("\n+" + string.Concat(Enumerable.Repeat(border, 12)));
            Console.WriteLine("\n\n" + line + line);
            Console.WriteLine("Press enter to continue.");
            Console.ReadLine();
        }
    }
}
