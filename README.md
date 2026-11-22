# Flashcards – LearnALanguage

A console app for learning words with flashcards. You group flashcards into **stacks**, then run **study sessions** where the app shows the start of each word and you type the rest. Every session is saved with its date and score, so you can see how you're doing over time.

Built with C# and .NET 9. It uses SQL Server, Dapper and Spectre.Console. I made it for [The C# Academy](https://www.thecsharpacademy.com/) Flashcards project.

## Features

- **Create a stack** – name a stack and add as many flashcards as you like. Duplicate stack names and empty names are rejected. Type `abort` to go back to the menu.
- **Display a stack** – pick a stack by name and see all its flashcards.
- **Display all stacks** – list every stack in a table.
- **Delete a stack** – remove a stack after you confirm. Its flashcards and study sessions are deleted with it (cascade delete).
- **Study session** – choose a stack. For each card you see the first two letters and type the whole word. You get one point per correct answer.
- **Study session history** – see the date and score of every past session.
- **Automatic database setup** – the `Stacks`, `Flashcards` and `StudySessions` tables are created the first time you run the app.

## Tech stack

| Area | Tool |
| --- | --- |
| Language / runtime | C# / .NET 9 |
| Database | SQL Server (Express or LocalDB) |
| Data access | [Dapper](https://github.com/DapperLib/Dapper) + Microsoft.Data.SqlClient |
| Console UI | [Spectre.Console](https://spectreconsole.net/) |
| Configuration | Microsoft.Extensions.Configuration (`appsettings.json`) |
| Dependency injection | Microsoft.Extensions.DependencyInjection |

## Project structure

The code is organised by feature, with a small domain layer and an infrastructure layer:

```
Kerem.Flashcards/
├── Domain/
│   ├── Entities/          Stack, Flashcard, StudySession, StackDTO
│   └── Interfaces/        IStackRepository, IStudySessionRepository
├── Features/              One class per use case (CreateStack, DeleteStack,
│                          GetStackByName, GetAllStacks, CreateStudySession,
│                          GetAllStudySessions) + StackMapper
├── Infrastructure/
│   ├── Persistence/       DapperDbContext (connection + database setup)
│   ├── Repositories/      Dapper implementations of the repositories
│   └── Utils/             Input validation
├── UI/                    ConsoleMenu (main menu loop)
├── DependencyInjection.cs Service registration
├── Program.cs             Entry point
└── appsettings.json       Connection string
```

### Database

```
Stacks          Flashcards               StudySessions
------          ----------               -------------
Id (PK)    ◄──  StackId (FK, cascade)    Id (PK)
Name            Id (PK)                  CreatedAt
                Name                     Score
                                         StackId (FK → Stacks, cascade)
```

Stacks are shown through `StackDTO`, so the database ids are never shown to the user.

## Getting started

### Prerequisites

- [.NET 9 SDK](https://dotnet.microsoft.com/download/dotnet/9.0)
- SQL Server. SQL Server Express, LocalDB or a Docker container all work.

### Setup

1. Clone the repository:
   ```bash
   git clone https://github.com/<your-username>/CodeReviews.Console.Flashcards.git
   cd CodeReviews.Console.Flashcards/Kerem.Flashcards/Kerem.Flashcards
   ```
2. Create an empty database called `FlashcardDb`. The app creates the tables itself.
3. Change the connection string in `appsettings.json` if your server isn't `localhost\SQLEXPRESS`:
   ```json
   "ConnectionStrings": {
     "DefaultConnection": "Data Source=localhost\\SQLEXPRESS;Initial Catalog=FlashcardDb;Integrated Security=true;TrustServerCertificate=true;"
   }
   ```
4. Run the app:
   ```bash
   dotnet run
   ```

> **Note:** `Program.cs` loads `appsettings.json` from a fixed path on my machine. On another machine, change that line to `.AddJsonFile("appsettings.json", optional: false)`, and make sure the file is copied to the output folder (`<CopyToOutputDirectory>PreserveNewest</CopyToOutputDirectory>` in the `.csproj`).

## Usage

Use the arrow keys and Enter to choose from the menu:

```
1. Create a stack
2. Display a stack
3. Display all stacks
4. Delete a stack
5. Start a study session
6. Display all study sessions
7. Exit
```

Example study session with a stack containing `hello`:

```
he
Please complete the word
> hello
1
```

## What I learned

- Using Dapper with raw SQL instead of a full ORM like Entity Framework
- Linking tables with foreign keys and cascade deletes
- Using DTOs so database details stay out of the UI
- Organising code by feature, with dependency injection and repository interfaces
- Building a nicer console UI with Spectre.Console

## Possible improvements

- Edit stacks and flashcards (rename, add or remove cards)
- Give flashcards a front and a back, e.g. a word and its translation
- Show the stack name in the study session history, plus a yearly report of sessions per month
- Add unit tests for the features and validation
