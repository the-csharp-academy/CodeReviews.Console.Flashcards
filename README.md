# Flashcards

Console based CRUD application to learn flashcards.

## How to setup
- Change in appsettings.json the two connection strings.
- "tcsa" and "master" should respectively reference to a database with the same name.

## Given Requirements:

- This is an application where the users will create Stacks of Flashcards.
- You'll need two different tables for stacks and flashcards. The tables should be linked by a foreign key.
- Stacks should have a unique name.
- Every flashcard needs to be part of a stack. If a stack is deleted, the same should happen with the flashcard.
- You should use DTOs to show the flashcards to the user without the Id of the stack it belongs to.
- When showing a stack to the user, the flashcard Ids should always start with 1 without gaps between them. If you have 10 cards and number 5 is deleted, the table should show Ids from 1 to 9.
- After creating the flashcards functionalities, create a "Study Session" area, where the users will study the stacks. All study sessions should be stored, with date and score.
- The study and stack tables should be linked. If a stack is deleted, it's study sessions should be deleted.
- The project should contain a call to the study table so the users can see all their study sessions. - This table receives insert calls upon each study session, but there shouldn't be update and delete calls to it.

## Features

- SQLServer database connection
- CRUD Database functions
- A console based UI using spectre

## Challenges

The whole setup with Docker and T-SQL felt not fun at all.  Like it was hard to get motivated.
I struggled with T-SQL. There is subtle syntax change here and there, and sometimes it's not subtle, it's just plain different. It got me really frustrated, especially since I did not found a reliable documentation. I used code bits for what I needed, without really improving or getting comfortable with the whole logic. 
Writing the app took me long but it wasn't so complicated. I stumble on a lot of edge cases that pushed me. But it always felt manageable.
I tried to get better at catching errors. I had many rewrites on how it worked. Still not fully confident with it.
I tried to implement Unit Test, but never find the right moment to do one. None of my method felt adapted to it, or maybe I am missing the point.
At the end, i did a lot of tutorial or read docs different topic (t-sql, linq, delegates, generics, docker, unit test) and retaining the information is really hard. I mean I don't use a lot of all those topics in one project. Just here and there. So i'm always like "Wait, how does it works again?"

## Lessons Learned

- Getting confident with creating more complex menu
- Learned about the existence of DTO
- Using a provider to manage connection string
- Learned about the existence of partial class
- I finally understood how delegates works, it clicked. I had a menu version using them before rewriting it.
- Getting confident with using source files like .csv
- Edge cases are everywhere

## Areas to Improve

- Managing my commit, I tend to forget stuff that should have been included in a previous commit
- T-SQL (felt like I "learned" the minimum here, wasn't motivated)
- A lot of area of error handling are remaining grey in my head, need to redo it many times
- Everything around container is still very foggy
	
## Resources Used
	
- The c# academy : LINQ course
- CodeCademy : course on delegate & generic 
- Spectre documentation
- T-Unit documentation (but never really used it)
- w3school.com for T-SQL basic syntax
- Docker Tuto getting started with container
- c-sharpcorner.com: Data Transfer Objects (DTOs) in C#
- Extensive use of copilot this time for (T-SQL, loads of architectural questions, quick syntax, mentoring when writing something that felt wrong or could be better written)


