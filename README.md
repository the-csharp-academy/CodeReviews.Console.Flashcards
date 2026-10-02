# Flashcards

Console application for creating flashcard stacks and studying them.
Built with C#, SQL Server, Dapper and Spectre.Console.

## Features

- Create and delete stacks and flashcards.
- Study a stack and save the session date and score.
- View study history, monthly session counts and average scores.
- Deleting a stack also deletes its cards and sessions.

## How to Run

1. Install the .NET 10 SDK, SQL Server Express LocalDB and SQL Server Management Studio (SSMS).
2. In SSMS, connect to `(localdb)\MSSQLLocalDB` using Windows Authentication and run:

   ```sql
   CREATE DATABASE Flashcards;
   ```

3. Check `appsettings.json`:

   ```json
   {
     "ConnectionStrings": {
       "DefaultConnection": "Server=(localdb)\\MSSQLLocalDB;Database=Flashcards;Trusted_Connection=true;"
     }
   }
   ```

   If using another SQL Server instance, replace the server name.

4. Run `dotnet run` from the project folder. Tables are created automatically.

Use arrow keys and Enter to navigate. Use Space to select items for deletion.
Create a stack, add cards, then choose **Study**. Enter `E` to stop a study session.
