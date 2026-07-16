# Flashcards

A .NET 10 console application for creating flashcard stacks, studying them, and reviewing study history in SQL Server LocalDB.

## Requirements

- [.NET 10 SDK](https://dotnet.microsoft.com/download/dotnet/10.0)
- SQL Server LocalDB with an instance named `MSSQLLocalDB`

## Run

```powershell
dotnet run --project Flashcards
```

On first run the app creates the `Flashcards` database and its schema. To use a different SQL Server connection, set `FLASHCARDS_CONNECTION_STRING` before starting the app.

```powershell
$env:FLASHCARDS_CONNECTION_STRING = "Server=(localdb)\MSSQLLocalDB;Database=Flashcards;Trusted_Connection=True;TrustServerCertificate=True"
dotnet run --project Flashcards
```

## Design

- `Stacks.Name` is unique.
- `Flashcards` and `StudySessions` have foreign keys to `Stacks` with cascade delete.
- Flashcards shown in the UI use `FlashcardDto`, which excludes the database `StackId`.
- Display IDs use SQL `ROW_NUMBER()`, so they are always consecutive without changing stable database IDs.
- Study sessions are append-only in the application: only insert and read operations are exposed.
- Both monthly reports use SQL Server `PIVOT` and always show January through December.
- SQL commands are parameterized; the only interpolated report fragment is a private, hard-coded aggregate expression.
