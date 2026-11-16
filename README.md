# Flashcards

A console-based flashcard application built with C# and SQL Server. It
allows users to organise flashcards into stacks, study a stack, record
study-session results, and review study history through reports.

The application uses [Spectre.Console](https://spectreconsole.net/) to
provide an interactive terminal interface.

## Features

### Stack management

-   Create, rename, and delete flashcard stacks.
-   Stack names are unique.
-   Deleting a stack also deletes its associated flashcards and study
    sessions through SQL Server cascading foreign keys.

### Flashcard management

-   Create, update, and delete flashcards.
-   Every flashcard belongs to a stack.
-   View flashcards with sequential display numbering.
-   The application displays the associated stack name when showing
    flashcards.

### Study sessions

-   Select a stack and work through its flashcards.
-   Enter an answer for each card and receive immediate feedback.
-   Record the number of correct answers, total cards, percentage score,
    start time, end time, and duration.
-   Study-session records are retained as history; the application
    provides no edit or delete functionality for them.

### Reports

-   View all study-session history.
-   Filter study-session history by stack.
-   View the number of study sessions per month for a selected year.
-   View average study-session scores per month for a selected year.

## Technology

-   C#
-   .NET 10
-   SQL Server
-   ADO.NET with `Microsoft.Data.SqlClient`
-   `Microsoft.Extensions.Hosting` and dependency injection
-   Spectre.Console
-   xUnit

## Project structure

``` text
Flashcards/
├── Flashcards/
│   ├── DTOs/
│   ├── Entities/
│   ├── Extensions/
│   ├── Interfaces/
│   ├── Repositories/
│   ├── Services/
│   ├── UI/
│   ├── Validation/
│   ├── Database/
│   │   ├── Schema/
│   │   └── Seed/
│   ├── appsettings.json
│   └── Program.cs
├── Flashcards.Test/
└── Flashcards.slnx
```

### Main responsibilities

  -----------------------------------------------------------------------
  Folder                              Responsibility
  ----------------------------------- -----------------------------------
  `DTOs`                              Objects used to transfer data
                                      between application layers and the
                                      UI

  `Entities`                          Application data models
                                      representing stacks, flashcards,
                                      and study sessions

  `Interfaces`                        Repository contracts

  `Repositories`                      SQL Server data access using
                                      ADO.NET

  `Services`                          Application logic and mapping
                                      between entities and DTOs

  `UI`                                Menus and terminal interaction

  `Validation`                        Input/model validation

  `Database/Schema`                   SQL scripts for creating the
                                      database tables

  `Flashcards.Test`                   Automated tests
  -----------------------------------------------------------------------

The application uses a repository/service structure. Repositories handle
database operations, services coordinate application logic, and the UI
handles user interaction. Dependencies are registered through the .NET
Generic Host.

## Requirements

To build and run the application, you need:

-   Windows with SQL Server LocalDB installed, or another accessible SQL
    Server instance.
-   .NET 10 SDK.
-   Visual Studio 2022 or another editor/terminal that supports .NET 10
    projects.
-   SQL Server Management Studio (SSMS), Azure Data Studio, or another
    SQL client to execute the database scripts.

## Database setup

Create appsettings.json in the root directory: `Flashcards/appsettings.json`. 
The default connection string should be configured for SQL Server LocalDB:

``` json
{
  "ConnectionStrings": {
    "MSSql": **Your connection string here**
  }
}
```

The application expects a database named `Flashcards` and a connection
string named `MSSql`.

1.  Start your SQL Server instance.
2.  Create a database named `Flashcards`.
3.  Open the SQL scripts in `Flashcards/Database/Schema/`.
4.  Execute the scripts in numerical order:
    -   `001_CreateStacks.sql`
    -   `002_CreateFlashcards.sql`
    -   `003_CreateStudySessions.sql`
5.  Check that the `Flashcards` project has a valid connection string in
    `appsettings.json`.

If you use a SQL Server instance other than LocalDB, update the `MSSql`
connection string to point to that instance. Use an appropriate
authentication method for your environment.

**Configuration note:** Do not commit passwords, access tokens, or other
secrets to source control. For a development setup that uses
credentials, configure them outside the committed application settings
file (for example, with .NET user secrets or environment variables).

## Run the application

From the solution directory, restore dependencies and run the console
application:

``` bash
dotnet restore
dotnet run --project Flashcards/Flashcards.csproj
```

The application starts at the main menu. Use the menu options to manage
stacks and flashcards, start study sessions, view reports, or exit.

## Run the tests

Run the test project from the solution directory:

``` bash
dotnet test Flashcards.Test/Flashcards.Test.csproj
```

The current xUnit tests cover invalid flashcard data (empty front/back
values) and an empty stack name. The test suite is intentionally small
at this stage and does not yet provide comprehensive coverage of the
application services or database repositories.

## Design notes

-   SQL Server constraints enforce unique stack names and the required
    relationships between stacks, flashcards, and study sessions.
-   Foreign keys use `ON DELETE CASCADE` so deleting a stack also
    removes its related flashcards and study sessions.
-   DTOs are used to shape data for the application and its UI. For
    example, flashcards retain their `StackId` so the application can
    look up and display the associated stack name.
-   Display numbers are generated by the application and are separate
    from database identity values.
-   Study-session history is intended to be read and added to, rather
    than edited or deleted.

## Known limitations

-   Answer checking currently compares answers after converting them to
    lowercase and removing spaces. It does not ignore punctuation or
    account for alternative valid answers.
-   Automated tests currently focus on a small part of validation logic.
    Service-level and database integration tests would improve
    confidence in future changes.
-   The default database configuration uses SQL Server LocalDB, which is
    intended for local development.

## Learning outcomes

This project was developed as a practical exercise in C# and
database-backed console application development. It provided experience
with:

-   C# classes, objects, and asynchronous methods.
-   Relational database design, primary keys, foreign keys, constraints,
    and cascading deletes.
-   SQL queries, joins, aggregation, and pivot-based monthly reports.
-   ADO.NET and parameterised SQL commands.
-   DTOs, repositories, services, and dependency injection.
-   Building interactive console interfaces with Spectre.Console.
-   Writing basic automated tests with xUnit.
