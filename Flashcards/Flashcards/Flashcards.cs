namespace Flashcards
{
    public class Flashcards
    {
        public static void FlashcardsTable()
        {
            using (var connection = Database.GetConnection())
            {
                var tableCmd = connection.CreateCommand();

                tableCmd.CommandText =
                @"IF OBJECT_ID('flashcards') IS NULL
                  BEGIN
                  CREATE TABLE flashcards(
                  id int IDENTITY(1,1) PRIMARY KEY,
                  stackId int NOT NULL,
                  question NVARCHAR(100) NOT NULL,
                  answer NVARCHAR(100) NOT NULL,
                  FOREIGN KEY (stackId)
                  REFERENCES stacks (id)
                  ON DELETE CASCADE);
                  END";
                tableCmd.ExecuteNonQuery();
            }
        }

        public static void AddFlashcards()
        {
            using (var connection = Database.GetConnection())
            {
                Console.WriteLine("Please insert stack ID for new flashcard");
                Stacks.ViewStack();

                string idInput = Console.ReadLine();
                int input;
                bool success = int.TryParse(idInput, out input);
                while (!success)
                {
                    Console.WriteLine("Please insert stack ID for new flashcard");
                    idInput = Console.ReadLine();
                    success = int.TryParse(idInput, out input);
                }
                bool stackExists = Stacks.StackExistsValidation(input);
                if (!stackExists)
                {
                    Console.WriteLine($"Stack with number {input} doesn't exist");
                    return;
                }
                Console.WriteLine("Please insert question for new flashcard");
                string question = Console.ReadLine();
                while (string.IsNullOrWhiteSpace(question))
                {
                    Console.WriteLine("Please insert question for new flashcard");
                    question = Console.ReadLine();
                }

                Console.WriteLine("Please insert answer for new flashcard");
                string answer = Console.ReadLine();
                while (string.IsNullOrWhiteSpace(answer))
                {
                    Console.WriteLine("Please insert answer for new flashcard");
                    answer = Console.ReadLine();
                }

                var tableCmd = connection.CreateCommand();

                tableCmd.CommandText =
                    @"INSERT INTO flashcards (stackId, question, answer)
                    VALUES (@stackId, @question, @answer);";

                tableCmd.Parameters.AddWithValue("@stackId", input);
                tableCmd.Parameters.AddWithValue("@answer", answer);
                tableCmd.Parameters.AddWithValue("@question", question);

                tableCmd.ExecuteNonQuery();
            }
        }

        public static int? ViewFlashcards()
        {
            using (var connection = Database.GetConnection())
            {
                Console.WriteLine("Please type ID of stack that you want to view");
                Stacks.ViewStack();
                string idInput = Console.ReadLine();
                int counter = 1;
                int input;
                bool success = int.TryParse(idInput, out input);
                while (!success)
                {
                    Console.WriteLine("Please type ID of stack that you want to view");
                    idInput = Console.ReadLine();
                    success = int.TryParse(idInput, out input);
                }
                bool stackExists = Stacks.StackExistsValidation(input);
                if (!stackExists)
                {
                    Console.WriteLine($"Stack with number {input} doesn't exist");
                    return null;
                }
                var tableCmd = connection.CreateCommand();

                tableCmd.CommandText =
                    @"SELECT * FROM flashcards
                    WHERE stackId = @stackId;";

                tableCmd.Parameters.AddWithValue("@stackId", input);

                using var reader = tableCmd.ExecuteReader();
                List<FlashcardDto> ShowFlashcards = new List<FlashcardDto>();
                while (reader.Read())
                {
                    FlashcardDto card = new FlashcardDto();
                    card.question = Convert.ToString(reader["question"]);
                    card.answer = Convert.ToString(reader["answer"]);
                    ShowFlashcards.Add(card);
                }
                foreach (var card in ShowFlashcards)
                {
                    Console.WriteLine(counter);
                    Console.WriteLine(card.question);
                    Console.WriteLine(card.answer);
                    counter++;
                }
                return input;
            }
        }

        public static void DeleteFlashcards()
        {
            using (var connection = Database.GetConnection())
            {
                int? stackId = ViewFlashcards();
                if (stackId == null)
                {
                    return;
                }
                int realFlashcardId = 0;
                int counter = 1;
                Console.WriteLine("Please type flashcard id that you want to delete");
                string idInput = Console.ReadLine();
                int input;
                bool success = int.TryParse(idInput, out input);
                while (!success)
                {
                    Console.WriteLine("Please type flashcard id that you want to delete");
                    idInput = Console.ReadLine();
                    success = int.TryParse(idInput, out input);
                }

                var tableCmd = connection.CreateCommand();

                tableCmd.CommandText =
                    @"SELECT id FROM flashcards
                     WHERE stackId = @stackId";

                tableCmd.Parameters.AddWithValue("@stackId", stackId);

                using var reader = tableCmd.ExecuteReader();
                while (reader.Read())
                {
                    Console.WriteLine(reader["id"].ToString());
                    if (counter == input)
                    {
                        realFlashcardId = Convert.ToInt32(reader["id"]);
                    }
                    counter++;
                }
                if (realFlashcardId == 0)
                {
                    Console.WriteLine("Wrong flashcard ID");
                    return;
                }
                reader.Close();

                tableCmd.CommandText =
                   @"DELETE FROM flashcards
                    WHERE id = @realFlashcardId";

                tableCmd.Parameters.AddWithValue("@realFlashcardId", realFlashcardId);

                tableCmd.ExecuteNonQuery();
            }
        }

        public static void UpdateFlashcards()
        {
            using (var connection = Database.GetConnection())
            {
                int? stackId = ViewFlashcards();
                if (stackId == null)
                {
                    return;
                }
                int realFlashcardId = 0;
                int counter = 1;
                Console.WriteLine("Please type flashcard id that you want to update");
                string idInput = Console.ReadLine();
                int input;
                bool success = int.TryParse(idInput, out input);
                while (!success)
                {
                    Console.WriteLine("Please type flashcard id that you want to update");
                    idInput = Console.ReadLine();
                    success = int.TryParse(idInput, out input);
                }

                Console.WriteLine("Please type new answer");
                string answer = Console.ReadLine();
                while (string.IsNullOrWhiteSpace(answer))
                {
                    Console.WriteLine("Please type new answer");
                    answer = Console.ReadLine();
                }

                Console.WriteLine("Please type new question");
                string question = Console.ReadLine();
                while (string.IsNullOrWhiteSpace(question))
                {
                    Console.WriteLine("Please type new question");
                    question = Console.ReadLine();
                }

                var tableCmd = connection.CreateCommand();

                tableCmd.CommandText =
                    @"SELECT id FROM flashcards
                    WHERE stackId = @stackId;";
                tableCmd.Parameters.AddWithValue("@stackId", stackId);
                using var reader = tableCmd.ExecuteReader();
                while (reader.Read())
                {
                    Console.WriteLine(reader["id"].ToString());
                    if (counter == input)
                    {
                        realFlashcardId = Convert.ToInt32(reader["id"]);
                        break;
                    }
                    counter++;
                }
                if (realFlashcardId == 0)
                {
                    Console.WriteLine("Wrong flashcard ID");
                    return;
                }
                reader.Close();

                tableCmd.CommandText =
                    @"UPDATE FLASHCARDS
                    SET answer = @answer,
                    question = @question
                    WHERE id = @realFlashcardId";
                tableCmd.Parameters.AddWithValue("@realFlashcardId", realFlashcardId);
                tableCmd.Parameters.AddWithValue("@answer", answer);
                tableCmd.Parameters.AddWithValue("@question", question);
                tableCmd.ExecuteNonQuery();
            }
        }

        public static List<Flashcard> GetFlashcardsForStack(int stackId)
        {
            using (var connection = Database.GetConnection())
            {
                var tableCmd = connection.CreateCommand();

                tableCmd.CommandText =
                @"SELECT * FROM flashcards
                    WHERE stackId = @stackId;";

                tableCmd.Parameters.AddWithValue("@stackId", stackId);

                using var reader = tableCmd.ExecuteReader();

                List<Flashcard> flashcards = new List<Flashcard>();
                while (reader.Read())
                {
                    Flashcard card = new Flashcard();
                    card.Id = Convert.ToInt32(reader["id"]);
                    card.Question = Convert.ToString(reader["question"]);
                    card.Answer = Convert.ToString(reader["answer"]);
                    flashcards.Add(card);
                }
                return flashcards;
            }
        }
    }
}