\# Flashcards



\*\*C# Console Application | SQL Server | Dapper | Spectre.Console\*\*



A console-based CRUD application for creating stacks of flashcards and studying them, built as part of the C# Academy learning path. This project builds on the Coding Tracker project, introducing SQL Server, linked tables with foreign keys, and DTOs (Data Transfer Objects).



\---



\## Requirements



| Feature | Status |

|---|---|

| Users can create Stacks of Flashcards | ✅ |

| Stacks and Flashcards stored in two separate tables, linked by a foreign key | ✅ |

| Stacks have a unique name | ✅ |

| Deleting a stack deletes its flashcards (cascading delete) | ✅ |

| DTOs used to display flashcards without exposing the stack's Id | ✅ |

| Flashcard Ids shown to the user always start at 1, with no gaps | ✅ |

| Study Session area where users study a stack | ✅ |

| Study sessions stored with date and score | ✅ |

| Study sessions linked to a stack, with cascading delete | ✅ |

| A view so users can see all their study sessions | ✅ |

| No update/delete operations against the study sessions table | ✅ |

| Handle all possible errors so the application never crashes | ✅ |

| \*\*Challenge:\*\* Report of sessions studied per month per stack | ✅ |

| \*\*Challenge:\*\* Report of average score per month per stack | ✅ |



\---



\## How It Works



The database connection string is stored in appsettings.json — update it there if you need to point the app at a different database.



From the main menu the user can:



\- Create a new stack

\- View all stacks

\- Delete a stack

\- Create a new flashcard within a stack

\- View all flashcards in a stack

\- Delete a flashcard from a stack

\- Study a stack — the app shows the front of each card, the user types a translation, and it's checked against the back (case-insensitive)

\- View study session results for a stack, and a yearly report broken down by month



Every flashcard belongs to a stack, and every study session belongs to a stack — deleting a stack cascades through both. Flashcards are always displayed with clean, sequential Ids (1, 2, 3...), regardless of gaps in the underlying database Ids caused by earlier deletions.



\---



\## Challenges



This project introduced SQL Server for the first time, after using SQLite in the previous two projects. Getting SQL Server Express and SSMS installed and connected was its own hurdle — the newer SSMS installer routes through the Visual Studio Installer, and the first connection attempt failed with an error that needed "Trust server certificate" enabled to resolve, which I later figured out.



Learning to write correct T-SQL instead of SQLite's dialect took some adjustment — `IDENTITY(1,1)` instead of `AUTOINCREMENT`, `NVARCHAR` instead of `TEXT`, and no `CREATE TABLE IF NOT EXISTS` shorthand. Foreign keys and `ON DELETE CASCADE` were new concepts, but became clear once I saw them working end-to-end: deleting a stack automatically removing its flashcards and study sessions without any extra code on my end.



The trickiest requirement was filtering and showing the results by month and by monthly average, but given this was the challenge task I can understand why it was more difficult compared to rest of the tasks.



I also ran into a subtle bug caused by variable shadowing — a local variable named `stacks` (a `List<Stacks>`) had the same name as a constructor parameter also named `stacks` (the table name string). Inside that method, every reference to `stacks` pointed to the list instead of the table name, and interpolating the list into a SQL string produced a nonsensical query with a stray backtick character in it, causing a confusing "Incorrect syntax" error from SQL Server that took some digging to trace back to its actual cause.



DTOs were a new concept, but simple to apply in practice — a stripped-down version of `Flashcards` without the `StackId`, used only when displaying data to the user, while the full object with `StackId` is still used internally for database operations.



\---



\## Areas to Improve



\- \*\*DRY Principle\*\* — Several methods repeat the same pattern of showing all stacks, then prompting for a stack Id via `CheckIdExists`, before performing an action. This could likely be consolidated further.

\- \*\*Average Score report\*\* — Implemented last, after days without working on the project, this humbled me a lot given I took hours trying to figure out again how every line works, so now I understand the importance of writing more clean code.



\---



\## What I Learned



\- Setting up and connecting to SQL Server Express/LocalDB via SSMS

\- Writing T-SQL, and how it differs from SQLite's dialect

\- Creating linked tables with foreign keys and `ON DELETE CASCADE`

\- Using DTOs to expose a different shape of the same data to the user

\- Solving a "renumber for display without touching the database" problem using a separate display counter

\- Working with native SQL Server `DATE` columns and .NET's `DateOnly` type, instead of storing dates as text

\- Building a manual pivot-style report using nested loops and LINQ, as an alternative to SQL Server's `PIVOT` syntax

\- Debugging a variable-shadowing bug and understanding how C# scoping rules can silently change what an identifier refers to



