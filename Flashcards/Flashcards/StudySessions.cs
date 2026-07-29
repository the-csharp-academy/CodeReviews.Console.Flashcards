namespace Flashcards
{
    public class StudySessions
    {
        public static void SessionsTable()
        {
            using (var connection = Database.GetConnection())
            {
                var tableCmd = connection.CreateCommand();

                tableCmd.CommandText =
                @"IF OBJECT_ID('sessions') IS NULL
                BEGIN
                CREATE TABLE sessions
                (id int IDENTITY(1,1) PRIMARY KEY,
                stackId int NOT NULL,
                goodAnswer int NOT NULL,
                totalQuestions int NOT NULL,
                startDateTime DATETIME2 NOT NULL,
                endDateTime DATETIME2 NOT NULL,
                FOREIGN KEY (stackId)
                REFERENCES stacks (id)
                ON DELETE CASCADE);
                END";
                tableCmd.ExecuteNonQuery();
            }
        }

        public static void StartSession()
        {
            Stacks.ViewStack();
            Console.WriteLine("Select stack that you want to use.");
            string stackIdInput = Console.ReadLine();
            int stackId = 0;
            bool success = int.TryParse(stackIdInput, out stackId);
            while (!success)
            {
                Console.WriteLine("Select stack that you want to use.");
                stackIdInput = Console.ReadLine();
                success = int.TryParse(stackIdInput, out stackId);
            }
            bool stackExists = Stacks.StackExistsValidation(stackId);
            if (!stackExists)
            {
                Console.WriteLine($"Stack with number {stackId} doesn't exist");
                return;
            }
            int goodAnswer = 0;
            int totalQuestions = 0;

            List<Flashcard> flashcards = Flashcards.GetFlashcardsForStack(stackId);
            var startDateTime = DateTime.Now;

            foreach (var card in flashcards)
            {
                Console.WriteLine(card.Question);
                Console.WriteLine("Write Answer");
                Console.WriteLine("Type \"exit\" if you want to stop");
                string userAnswer = Console.ReadLine();
                while (string.IsNullOrWhiteSpace(userAnswer))
                {
                    Console.WriteLine("Write Answer");
                    Console.WriteLine("Type \"exit\" if you want to stop");
                    userAnswer = Console.ReadLine();
                }
                if (userAnswer.Equals("exit", StringComparison.OrdinalIgnoreCase))
                {
                    break;
                }
                if (userAnswer.Equals(card.Answer, StringComparison.OrdinalIgnoreCase))
                {
                    Console.WriteLine("Good answer!");
                    goodAnswer++;
                }
                else
                {
                    Console.WriteLine("Bad Answer");
                }

                totalQuestions++;
            }

            var endDateTime = DateTime.Now;

            SaveSession(stackId, goodAnswer, totalQuestions, startDateTime, endDateTime);

            Console.WriteLine($"You got {goodAnswer} points out of {totalQuestions} questions\n" +
                $"Session length: {endDateTime - startDateTime}");
        }

        public static void SaveSession(int stackId, int goodAnswer, int totalQuestions, DateTime startDateTime, DateTime endDateTime)
        {
            using (var connection = Database.GetConnection())
            {
                var tableCmd = connection.CreateCommand();

                tableCmd.CommandText =
                    @"INSERT INTO sessions (stackId, goodAnswer, totalQuestions, startDateTime, endDateTime)
                      VALUES(@stackId, @goodAnswer, @totalQuestions, @startDateTime, @endDateTime);";
                tableCmd.Parameters.AddWithValue("@stackId", stackId);
                tableCmd.Parameters.AddWithValue("@goodAnswer", goodAnswer);
                tableCmd.Parameters.AddWithValue("@totalQuestions", totalQuestions);
                tableCmd.Parameters.AddWithValue("@startDateTime", startDateTime);
                tableCmd.Parameters.AddWithValue("@endDateTime", endDateTime);

                tableCmd.ExecuteNonQuery();
            }
        }

        public static void ViewSessions()
        {
            using (var connection = Database.GetConnection())
            {
                var tableCmd = connection.CreateCommand();

                tableCmd.CommandText =
                    @"SELECT * FROM sessions;";

                string TableNames = String.Format("|{0,10}|{1,10}|{2,15}|{3,10}|{4,10}|", "StackId", "Score", "Total questions", "Duration", "Date");

                using var reader = tableCmd.ExecuteReader();
                Console.WriteLine(TableNames);
                while (reader.Read())
                {
                    string startDate = reader["startDateTime"].ToString();
                    DateTime startDateCalculation = DateTime.Parse(startDate);

                    string endDate = reader["endDateTime"].ToString();
                    DateTime endDateCalculation = DateTime.Parse(endDate);

                    TimeSpan duration = endDateCalculation - startDateCalculation;

                    Console.WriteLine(String.Format("|{0,10}|{1,10}|{2,15}|{3,10}|{4,10}",
                        reader["stackId"].ToString(),
                        reader["goodAnswer"].ToString(),
                        reader["totalQuestions"].ToString(),
                        (duration.ToString(@"hh\:mm\:ss\.ff")), (startDateCalculation)));
                }
            }
        }
    }
}