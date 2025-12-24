# Flashcard App

Console application written with C# 14/.NET 10 using JetBrains Rider on Ubuntu 25.10.

This is a simple console application that allows you to create stacks of flashcards which in turn you can study.

Created following the curriculum of [C# Academy](https://www.thecsharpacademy.com/)

[Flashcard App Requirements](https://www.thecsharpacademy.com/project/14/flashcards)

# Features 
- Creates a SqlServer database upon application initialization if it has not already been created.
- Creates all relevant tables in the database if they do not already exist upon application initialization.
- Allows the user create stacks of flashcards on different subjects and gives the user the ability to create a study session in which to study their flashcards.
- Implements logging and unit tests as well as handling exceptions to prevent application from crashing.

# Challenges Faced When Implementing This Project
- SqlServer. Moving from Sqlite in the previous projects to SqlServer in this one was at time challenging. Mostly using SqlServer on a GNU/Linux operating system as well as the differences in how databases are created as well as differences in SQL syntax in SqlServer (Transact-SQL) as opposed to Sqlite.
- Database Table Relationships. Gaining understanding of database table relationships.
- Keeping this concise and focused. In previous projects there has been a tendency on my part to try to implement more functionality than needed, which oftentimes has caused me to lose sight of exactly what is supposed to be learned with each project. This time around I tried to stay closer to the project requirements.

# What Was Learned Implementing This Project
- Learned a decent amount about Transact-SQL which is the SQL dialect used by Microsoft's SQL Server.
- Learned a few more SQL Queries to extract information from the database such as how to return the id of a newly saved "row" in the database.
- Learned how SQL Server handles different datatypes in comparision to how Sqlite handled them. For example SQL Server has datatypes that in my opinion are perfect for "mapping" to C# datatypes: ex: TimeSpan can be mapped to the SQL Server datatype Time, whereas with Sqlite a TimeSpan would have to be saved a string and then parsed when retrieving that particular column from the database.
- Overall learned more about C# and databases in general.

# Areas To Improve Upon
- Everything.

# Technologies Used
- [Spectre.Console](https://spectreconsole.net/)
- [Dapper](https://github.com/DapperLib/Dapper)
- [Microsoft SQL Server](https://learn.microsoft.com/en-us/sql/sql-server/?view=sql-server-ver17)
- [Microsoft.Data.SqlClient](https://github.com/dotnet/sqlclient)
- [Serilog](https://serilog.net/)
- [Microsoft.Extensions.Configuration](https://www.nuget.org/packages/Microsoft.Extensions.Configuration)
- [XUnit](https://xunit.net/?tabs=cs)
- [Moq](https://github.com/devlooped/moq)

# Instructions Before Usage
- Database name is flashcardsDB
- Simply add your connection string for SQL Server to the appsettings.json file and then build and run the application.

