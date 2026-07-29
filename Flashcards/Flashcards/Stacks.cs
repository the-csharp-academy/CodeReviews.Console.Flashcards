using Microsoft.Data.SqlClient;

namespace Flashcards
{
    public class Stacks
    {
        public static void CreateStack()
        {
            using (var connection = Database.GetConnection())
            {
                var tableCmd = connection.CreateCommand();

                tableCmd.CommandText =

                @"IF OBJECT_ID('stacks') IS NULL
                BEGIN
                CREATE TABLE stacks (
                id int IDENTITY(1,1) PRIMARY KEY ,
                name NVARCHAR(100) NOT NULL UNIQUE);
                END";
                tableCmd.ExecuteNonQuery();
            }
        }

        public static void NewStack()
        {
            using (var connection = Database.GetConnection())
            {
                Console.WriteLine("Enter stack name");
                string name = Console.ReadLine();
                while (string.IsNullOrWhiteSpace(name))
                {
                    Console.WriteLine("Enter stack name");
                    name = Console.ReadLine();
                }

                var tableCmd = connection.CreateCommand();

                tableCmd.CommandText =
                @"INSERT INTO stacks
                (name) VALUES (@name) ;";

                tableCmd.Parameters.AddWithValue("@name", name);
                try
                {
                    tableCmd.ExecuteNonQuery();
                }
                catch (SqlException ex) when (ex.Number == 2627 || ex.Number == 2601)
                {
                    Console.WriteLine("Stack with this name already exists");
                }
            }
        }

        public static void UpdateStack()
        {
            using (var connection = Database.GetConnection())
            {
                ViewStack();
                Console.WriteLine("Please select id that you want to update");

                string idInput = Console.ReadLine();

                int input;

                bool success = int.TryParse(idInput, out input);
                while (!success)
                {
                    Console.WriteLine("Please select id that you want to update");
                    idInput = Console.ReadLine();
                    success = int.TryParse(idInput, out input);
                }
                bool stackExists = StackExistsValidation(input);
                if (!stackExists)
                {
                    Console.WriteLine($"Stack with number {input} doesn't exist.");
                    return;
                }
                Console.WriteLine($"Please type new name for stack {input}");

                string name = Console.ReadLine();
                while (string.IsNullOrWhiteSpace(name))
                {
                    Console.WriteLine($"Please type new name for stack {input}");
                    name = Console.ReadLine();
                }

                var tableCmd = connection.CreateCommand();

                tableCmd.CommandText =
                @"UPDATE stacks
                SET name = @name
                WHERE id = @id;";

                tableCmd.Parameters.AddWithValue("@id", input);
                tableCmd.Parameters.AddWithValue("@name", name);
                try
                {
                    tableCmd.ExecuteNonQuery();
                }
                catch (SqlException ex) when (ex.Number == 2627 || ex.Number == 2601)
                {
                    Console.WriteLine("Stack with this name already exists");
                }
            }
        }

        public static void DeleteStack()
        {
            using (var connection = Database.GetConnection())
            {
                ViewStack();

                Console.WriteLine("Please select id that you want to delete");
                string stringInput = Console.ReadLine();

                int input;

                bool success = int.TryParse(stringInput, out input);
                while (!success)
                {
                    Console.WriteLine("Please select id that you want to delete");
                    stringInput = Console.ReadLine();
                    success = int.TryParse(stringInput, out input);
                }
                bool stackExists = StackExistsValidation(input);
                if (!stackExists)
                {
                    Console.WriteLine($"Stack with number {input} doesn't exist");
                    return;
                }

                var tableCmd = connection.CreateCommand();

                tableCmd.CommandText =
                    @"DELETE FROM stacks
                    WHERE id = @id;";

                tableCmd.Parameters.AddWithValue("@id", input);

                tableCmd.ExecuteNonQuery();
                Console.WriteLine($"Stack {input} has been deleted");
            }
        }

        public static void ViewStack()
        {
            using (var connection = Database.GetConnection())
            {
                var tableCmd = connection.CreateCommand();

                tableCmd.CommandText =
                @"SELECT * FROM stacks
                ;";

                using var reader = tableCmd.ExecuteReader();

                while (reader.Read())
                {
                    Console.WriteLine(
                        reader["id"].ToString()
                        + '.' +
                        reader["name"].ToString());
                }
            }
        }

        public static bool StackExistsValidation(int id)
        {
            using (var connection = Database.GetConnection())
            {
                var tableCmd = connection.CreateCommand();
                tableCmd.CommandText =
                    @"SELECT COUNT (id)
                    FROM stacks
                    WHERE id = @id";

                tableCmd.Parameters.AddWithValue("@id", id);
                object result = tableCmd.ExecuteScalar();
                int count = Convert.ToInt32(result);
                return count >= 1;
            }
        }
    }
}