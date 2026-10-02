using Flashcards.Controllers;
using Flashcards.DTOs;
using Spectre.Console;
using System;
using System.Collections.Generic;
using System.Text;

namespace Flashcards.Menus
{
    public class StudyMenu
    {
        private readonly CardStackController cardStackController;
        private readonly FlashcardController flashcardController;
        private readonly StudySessionController studySessionController;

        public StudyMenu(CardStackController cardStackController, FlashcardController flashcardController, StudySessionController studySessionController)
        {
            this.flashcardController = flashcardController;
            this.cardStackController = cardStackController;
            this.studySessionController = studySessionController;
        }

        public void Run()
        {
            var data = cardStackController.ReadCardStacks().ToList();
            while (true)
            {
                Console.Clear();

                var action = UI.GetActions(new List<string>
                {
                    "Choose stack",
                    "View study sessions",
                    "View monthly report",
                    "View average scores",
                    "Exit"
                });

                if (action == "Exit")
                    return;
                if (action == "Choose stack")
                    ManageStacks(data);
                if (action == "View study sessions")
                    ViewStudySessions(data);
                if (action == "View monthly report")
                    ViewMonthlyReport();
                if (action == "View average scores")
                    ViewAverageScores();
            }

        }
        private void ManageStacks(List<CardStackDTO> data)
        {
            if (data.Count == 0)
                return;

            var list = data.Select(c => c.Name).ToList();
            var stackName = UI.GetActions(list);
            var stackOwner = data.First(c => c.Name == stackName);
            var studySession = new StudySession(flashcardController, studySessionController);
            studySession.StartGame(stackOwner);
        }

        private void ViewStudySessions(List<CardStackDTO> data)
        {
            var sessions = studySessionController.ReadSessions(data);
            if (sessions.Count == 0)
            {
                UI.PrintMessage("No study sessions found.");
                return;
            }
            var table = new Table()
                .AddColumn("Card stack name")
                .AddColumn("Time")
                .AddColumn("Score");

            foreach (var session in sessions)
            {
                table.AddRow(Markup.Escape(session.CardStackName), session.Time.ToString(), session.Score.ToString());
            }

            AnsiConsole.Write(table);
            Console.WriteLine("Press Enter to return to the study menu.");
            Console.ReadLine();
        }

        private void ViewMonthlyReport()
        {
            var year = UI.AskNumber("Enter the year (1-2100) for which to view the monthly report:", 1, 2100);
            var table = new Table()
                .AddColumn("Stackname")
                .AddColumn("January")
                .AddColumn("February")
                .AddColumn("March")
                .AddColumn("April")
                .AddColumn("May")
                .AddColumn("June")
                .AddColumn("July")
                .AddColumn("August")
                .AddColumn("September")
                .AddColumn("October")
                .AddColumn("November")
                .AddColumn("December");
            var monthlySessions = studySessionController.GetMonthlySessions(year);
            foreach (var session in monthlySessions)
            {
                table.AddRow(
                    Markup.Escape(session.StackName),
                    session.January.ToString(),
                    session.February.ToString(),
                    session.March.ToString(),
                    session.April.ToString(),
                    session.May.ToString(),
                    session.June.ToString(),
                    session.July.ToString(),
                    session.August.ToString(),
                    session.September.ToString(),
                    session.October.ToString(),
                    session.November.ToString(),
                    session.December.ToString()
                );
            }
            AnsiConsole.Write(table);
            Console.WriteLine("Press Enter to return to the study menu.");
            Console.ReadLine();
        }

        private void ViewAverageScores()
        {
            var year = UI.AskNumber("Enter the year (1-2100) for which to view the average scores:", 1, 2100);
            var month = UI.AskNumber("Enter the month (1-12) for which to view the average scores:", 1, 12);
   
            var averageScores = studySessionController.GetAverageScores(month, year);
            if (averageScores.Count == 0)
            {
                UI.PrintMessage("No average scores found for the specified month.");
                return;
            }
            var table = new Table()
                .AddColumn("Stackname")
                .AddColumn("Month")
                .AddColumn("Average Score");
            foreach (var score in averageScores)
            {
                table.AddRow(
                    Markup.Escape(score.StackName),
                    score.SessionMonth?.ToString() ?? "—",
                    score.AverageScore?.ToString("F2") ?? "—");
            }
            AnsiConsole.Write(table);
            Console.WriteLine("Press Enter to return to the study menu.");
            Console.ReadLine();
        }
    }
}
