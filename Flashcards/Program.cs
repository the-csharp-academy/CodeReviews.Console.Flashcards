using Flashcards;
using Microsoft.Data.SqlClient;

const string defaultConnection = "Server=(localdb)\\MSSQLLocalDB;Database=Flashcards;Trusted_Connection=True;TrustServerCertificate=True";
var connectionString = Environment.GetEnvironmentVariable("FLASHCARDS_CONNECTION_STRING") ?? defaultConnection;

try
{
    var database = new Database(connectionString);
    await database.InitializeAsync();
    await new ConsoleApp(new FlashcardRepository(connectionString), new StudyRepository(connectionString)).RunAsync();
}
catch (SqlException exception)
{
    Console.Error.WriteLine($"Database error: {exception.Message}");
    Console.Error.WriteLine("Confirm that SQL Server LocalDB is installed and the MSSQLLocalDB instance can start.");
    Environment.ExitCode = 1;
}

namespace Flashcards
{
    public sealed class ConsoleApp(FlashcardRepository cards, StudyRepository studies)
    {
        private static readonly string[] MonthNames =
            ["Jan", "Feb", "Mar", "Apr", "May", "Jun", "Jul", "Aug", "Sep", "Oct", "Nov", "Dec"];

        public async Task RunAsync()
        {
            while (true)
            {
                Header("FLASHCARDS");
                Console.WriteLine("1. Manage stacks\n2. Manage flashcards\n3. Study\n4. View study sessions\n5. Reports\n0. Exit");
                switch (Read("Choose an option"))
                {
                    case "1": await ManageStacksAsync(); break;
                    case "2": await ManageCardsAsync(); break;
                    case "3": await StudyAsync(); break;
                    case "4": await ShowSessionsAsync(); break;
                    case "5": await ReportsAsync(); break;
                    case "0": return;
                    default: Message("Please enter one of the listed options."); break;
                }
            }
        }

        private async Task ManageStacksAsync()
        {
            while (true)
            {
                Header("STACKS");
                await PrintStacksAsync();
                Console.WriteLine("\n1. Create\n2. Rename\n3. Delete\n0. Back");
                try
                {
                    switch (Read("Choose an option"))
                    {
                        case "1":
                            var name = ReadRequired("New stack name");
                            await cards.CreateStackAsync(name);
                            Message("Stack created.");
                            break;
                        case "2":
                            var rename = await ChooseStackAsync();
                            if (rename is not null)
                            {
                                await cards.RenameStackAsync(rename.Id, ReadRequired("New name"));
                                Message("Stack renamed.");
                            }
                            break;
                        case "3":
                            var delete = await ChooseStackAsync();
                            if (delete is not null && Read("Type DELETE to confirm") == "DELETE")
                            {
                                await cards.DeleteStackAsync(delete.Id);
                                Message("Stack and its cards and study sessions were deleted.");
                            }
                            break;
                        case "0": return;
                        default: Message("Invalid option."); break;
                    }
                }
                catch (SqlException ex) when (ex.Number is 2601 or 2627)
                {
                    Message("Stack names must be unique.");
                }
            }
        }

        private async Task ManageCardsAsync()
        {
            var stack = await ChooseStackAsync();
            if (stack is null) return;
            while (true)
            {
                Header($"FLASHCARDS — {stack.Name}");
                PrintCards(await cards.GetCardsAsync(stack.Id));
                Console.WriteLine("\n1. View first cards\n2. Create\n3. Edit\n4. Delete\n5. Change stack\n0. Back");
                switch (Read("Choose an option"))
                {
                    case "1":
                        if (int.TryParse(Read("How many cards"), out var count) && count > 0)
                            PrintCards(await cards.GetCardsAsync(stack.Id, count), pause: true);
                        else Message("Enter a positive number.");
                        break;
                    case "2":
                        await cards.AddCardAsync(stack.Id, ReadRequired("Front"), ReadRequired("Back"));
                        Message("Flashcard created.");
                        break;
                    case "3":
                        if (TryReadId(out var editId))
                        {
                            var updated = await cards.UpdateCardAsync(stack.Id, editId, ReadRequired("New front"), ReadRequired("New back"));
                            Message(updated ? "Flashcard updated." : "That displayed ID does not exist.");
                        }
                        break;
                    case "4":
                        if (TryReadId(out var deleteId))
                            Message(await cards.DeleteCardAsync(stack.Id, deleteId) ? "Flashcard deleted." : "That displayed ID does not exist.");
                        break;
                    case "5":
                        var changed = await ChooseStackAsync();
                        if (changed is not null) stack = changed;
                        break;
                    case "0": return;
                    default: Message("Invalid option."); break;
                }
            }
        }

        private async Task StudyAsync()
        {
            var stack = await ChooseStackAsync();
            if (stack is null) return;
            var deck = await cards.GetCardsAsync(stack.Id);
            if (deck.Count == 0) { Message("This stack has no cards yet."); return; }

            var shuffled = deck.OrderBy(_ => Random.Shared.Next()).ToList();
            var score = 0;
            var attempted = 0;
            foreach (var card in shuffled)
            {
                Header($"STUDY — {stack.Name} ({attempted + 1}/{shuffled.Count})");
                Console.WriteLine(card.Front);
                var answer = Read("Your answer (0 ends the session)");
                if (answer == "0") break;
                attempted++;
                if (string.Equals(answer.Trim(), card.Back.Trim(), StringComparison.OrdinalIgnoreCase))
                {
                    score++;
                    Console.ForegroundColor = ConsoleColor.Green;
                    Console.WriteLine("Correct!");
                }
                else
                {
                    Console.ForegroundColor = ConsoleColor.Yellow;
                    Console.WriteLine($"Not quite. Correct answer: {card.Back}");
                }
                Console.ResetColor();
                Pause();
            }
            await studies.AddSessionAsync(stack.Id, score, attempted);
            Message($"Session saved: {score}/{attempted} correct.");
        }

        private async Task ShowSessionsAsync()
        {
            Header("STUDY SESSIONS");
            var sessions = await studies.GetSessionsAsync();
            if (sessions.Count == 0) Console.WriteLine("No sessions recorded.");
            else
            {
                Console.WriteLine($"{"Id",-5}{"Stack",-22}{"Date",-20}{"Score",-12}{"Percent",8}");
                foreach (var s in sessions)
                    Console.WriteLine($"{s.Id,-5}{Clip(s.StackName, 20),-22}{s.StudiedAt,-20:g}{$"{s.Score}/{s.TotalCards}",-12}{Percent(s.Score, s.TotalCards),7:0.#}%");
            }
            Pause();
        }

        private async Task ReportsAsync()
        {
            Header("REPORTS");
            if (!int.TryParse(Read("Year (YYYY)"), out var year) || year is < 1 or > 9999)
            { Message("Enter a valid four-digit year."); return; }
            Console.WriteLine("1. Sessions per month\n2. Average score per month");
            IReadOnlyList<MonthlyReportRow> report;
            var choice = Read("Choose a report");
            if (choice == "1") report = await studies.GetSessionCountReportAsync(year);
            else if (choice == "2") report = await studies.GetAverageScoreReportAsync(year);
            else { Message("Invalid report."); return; }
            PrintReport(report, choice == "2");
            Pause();
        }

        private async Task<Stack?> ChooseStackAsync()
        {
            Header("CHOOSE A STACK");
            var all = await cards.GetStacksAsync();
            if (all.Count == 0) { Message("Create a stack first."); return null; }
            foreach (var stack in all) Console.WriteLine($"- {stack.Name}");
            var name = Read("Stack name (0 cancels)");
            if (name == "0") return null;
            var found = await cards.FindStackAsync(name);
            if (found is null) Message("Stack not found. Names are matched without regard to case.");
            return found;
        }

        private async Task PrintStacksAsync()
        {
            var stacks = await cards.GetStacksAsync();
            if (stacks.Count == 0) Console.WriteLine("No stacks yet.");
            else foreach (var stack in stacks) Console.WriteLine($"- {stack.Name}");
        }

        private static void PrintCards(IReadOnlyList<FlashcardDto> items, bool pause = false)
        {
            if (items.Count == 0) Console.WriteLine("No flashcards yet.");
            else
            {
                Console.WriteLine($"{"Id",-5}{"Front",-36}{"Back",-36}");
                foreach (var card in items)
                    Console.WriteLine($"{card.DisplayId,-5}{Clip(card.Front, 34),-36}{Clip(card.Back, 34),-36}");
            }
            if (pause) Pause();
        }

        private static void PrintReport(IReadOnlyList<MonthlyReportRow> rows, bool percent)
        {
            Console.WriteLine($"\n{"Stack",-18}{string.Concat(MonthNames.Select(m => $"{m,8}"))}");
            foreach (var row in rows)
            {
                Console.Write($"{Clip(row.StackName, 16),-18}");
                foreach (var value in row.Months) Console.Write(percent ? $"{value,7:0.#}%" : $"{value,8:0}");
                Console.WriteLine();
            }
            if (rows.Count == 0) Console.WriteLine("No stacks available.");
        }

        private static bool TryReadId(out int id)
        {
            if (int.TryParse(Read("Displayed flashcard ID"), out id) && id > 0) return true;
            Message("Enter a positive displayed ID.");
            return false;
        }

        private static string ReadRequired(string prompt)
        {
            while (true)
            {
                var value = Read(prompt).Trim();
                if (value.Length > 0) return value;
                Console.WriteLine("A value is required.");
            }
        }

        private static string Read(string prompt)
        {
            Console.Write($"{prompt}: ");
            return Console.ReadLine() ?? "0";
        }

        private static void Header(string title)
        {
            if (!Console.IsOutputRedirected) Console.Clear();
            Console.ForegroundColor = ConsoleColor.Cyan;
            Console.WriteLine($"=== {title} ===\n");
            Console.ResetColor();
        }

        private static void Message(string text) { Console.WriteLine($"\n{text}"); Pause(); }
        private static void Pause()
        {
            Console.WriteLine("\nPress any key to continue...");
            if (!Console.IsInputRedirected) Console.ReadKey(true);
        }
        private static string Clip(string value, int length) => value.Length <= length ? value : value[..(length - 1)] + "…";
        private static decimal Percent(int score, int total) => total == 0 ? 0 : score * 100m / total;
    }
}
