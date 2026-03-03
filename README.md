# Flashcard application
This is a learning project made for and following the requirments set out in [this project page](https://thecsharpacademy.com/project/14/flashcards).  
This project allows the user to create, edit, delete, and view different 'stacks' that act as containers for flashcards, as well as individual flashcards which are assigned
to their respective stacks.  
The app also features a study area which allows users to study the flashcards within their chosen stack. This works off a self correction system where the user is in charge of
marking each individual card as studied.

# Features
* The Spectre.Console library is used for a cleaner console UI.  
  ![Image displaying the application's main menu](https://i.imgur.com/DKq9eew.png)  
* The application features a stack and flashcards section for executing [CRUD](https://en.wikipedia.org/wiki/Create,_read,_update_and_delete) operations.  
* The application features a study session for studying a chosen stack, as well as viewing past study sessions.  
  ![Image showing the study section's main menu](https://i.imgur.com/AxV9VSi.png)  

# Setup
You'll need to install [Sql server 2025 and LocalDb](https://www.microsoft.com/en-us/sql-server/sql-server-downloads). Once you download, setup, and install localDb you should
have no more concerns with the setup. The application should automatically create a new localDb instance, and start it on each start up.  

# Resources Used
[.NET (10.0)](https://learn.microsoft.com/en-us/dotnet/)  
[Dapper (2.1.66)](https://www.learndapper.com) - ORM  
[Microsoft Sql Server 2025 (17.0.1000.7)](https://www.microsoft.com/en-us/sql-server/sql-server-downloads)  
[Microsoft.Data.SqlClient](https://learn.microsoft.com/en-us/sql/connect/ado-net/introduction-microsoft-data-sqlclient-namespace?view=sql-server-ver17) - Database  
[Microsoft.Extensions.Configuration (10.0.2)](https://learn.microsoft.com/en-us/dotnet/api/microsoft.extensions.configuration?view=net-10.0-pp) - Configuration  
[Microsoft.Extensions.Configuration.EnviromentVariables (10.0.2)](https://learn.microsoft.com/en-us/dotnet/api/microsoft.extensions.configuration.environmentvariablesextensions?view=net-10.0-pp)  
[Microsoft.Extensions.Configuration.Json (10.0.2)](https://learn.microsoft.com/en-us/dotnet/api/microsoft.extensions.configuration.json?view=net-8.0-pp)  
[Microsoft.Extensions.Logging (10.0.2)](https://learn.microsoft.com/en-us/dotnet/core/extensions/logging/overview?tabs=command-line) - Logging  
[Microsoft.Extensions.Logging.Abstractions (10.0.2)](https://learn.microsoft.com/en-us/dotnet/api/microsoft.extensions.logging.abstractions?view=net-10.0-pp)  
[Microsoft.Extensions.Logging.Console (10.0.2)](https://learn.microsoft.com/en-us/dotnet/api/microsoft.extensions.logging.console?view=net-8.0-pp)  

# Personal thoughts
I liked making the project, it was a little weird at first to figure out how to use Sql server instead of Sqlite, I especially had some problems with setting up LocalDb, but once
everything was set up I enjoyed using it. I also tried to use some dependency injection in this project, which I liked learning, although I'm not sure if the actual code i wrote for
this is any good. I tried going with an idea of three different areas of the app for each feature, with the 'stack area', 'flashcard area', and 'study area' all being accessed
via the main menu. Sadly, I wasn't able to do the challenges for this project. I had planned on doing the challenges, but I had to go out of town for a week and ended up leaving
earlier than I expected, so I didn't get to even start them before I left. Instead of focusing on the challenges, I decided it would be better to focus on just getting the project
done before I left, but I ended up still having a couple of small tweaks to do so I couldn't get it submitted in time, and now that I'm back, I honestly just don't want to do the
challenges, and would rather just finish up this project so I can start fresh on the next one. Maybe in the future I might come back and finish the challenge on this project, but
I think for now I'd rather just move on to the next thing. Overall, I enjoyed this project, and I'm looking forward to doing the next one, especially after not doing any projects
for a week.
