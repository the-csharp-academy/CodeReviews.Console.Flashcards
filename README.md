# Flashcards

Console-based flashcards application where users can create stacks of flashcards, study them, and keep track of their learning progress.
Developed using C#/.NET 9, SQL Server LocalDB, Dapper, and Spectre.Console.

## Given Requirements

- [x] Users can create, view, update, and delete stacks of flashcards.
- [x] Each flashcard belongs to a stack, and deleting a stack also removes its flashcards.
- [x] Stacks must have unique names.
- [x] Flashcards are shown to the user through DTOs so the stack id is not exposed unnecessarily.
- [x] Flashcard IDs are displayed as a continuous sequence starting from 1.
- [x] Users can study their stacks and record study sessions with a score and date.
- [x] Study sessions are linked to stacks, and deleting a stack removes related sessions.
- [x] The application uses a real database and initializes its tables automatically on startup.

## Features

- SQL Server LocalDB database connection
  - The app creates the database and required tables when it starts, if they do not already exist.

- Stack management
  - Create, read, update, and delete stacks with validation for duplicate names.

- Flashcard management
  - Add, view, edit, and remove flashcards inside a selected stack.

- Study mode
  - Study flashcards from a chosen stack and record your performance in a study session.

- Study history
  - Review completed sessions with scores and timestamps.

- Clean console UI
  - Navigation is handled through a polished terminal interface using Spectre.Console.

- Unit and integration tests
  - Both tests are written with the help of NSubstitute library and Nunit 3 framework.

## What I've Learned

- Working with SQL Server and creating linked tables with foreign keys.
- Using DTOs to shape data differently for the presentation layer.
- Implementing CRUD logic in a console application with separation between controllers, views, repositories, and models.
- Using Dapper for data access and parameterized SQL.
- Writing unit and integration tests for repository and controller behavior.

## How to Run

1. Install SQL Server LocalDB.
2. Restore the project dependencies:
   - `dotnet restore`
3. Run the application from the repository root:
   - `dotnet run --project Flashcards/Flashcards.csproj`
4. Run the tests:
   - `dotnet test Flashcards.Tests/Flashcards.Tests.csproj`
