# Flashcards.m1chael888
This is a simple console app that can be used to create sets of flashcards. Sets can be studied and a session history will be saved displaying the date and score of each session. Developed in C# using SQL Server and Dapper for data access.

# How it works
* Before starting the app, please change the appSettingsExample.json file according to the image below. You will need to change the file name to "appSettings.json" and change the connection string to your local SQL SERVER connection string for the app to work.
<img width="739" height="98" alt="image" src="https://github.com/user-attachments/assets/7d2ffea7-82ba-4322-a49a-ad5599edda7f" />

* The app starts with a main menu that has 3 options. Study, manage stacks and exit.

* Study
  - The study menu allows you to either study from an existing stack of cards or view your study history. When choosing to study a stack, you will be showed the front of a card and be asked to press any key when you are ready to see the answer. Scoring is calculated by         whether you knew the answer or not, which you will select before being showed the next card. If you would like to end the session early, you  can select back to menu instead of answering. You will be showed cards in a random order until the end of the stack is reached.     When viewing session history, you will be showed the date, score and stack of each study session you have completed so far

* Manage Stacks
  - The manage stacks menu allows you to create and manage both stacks and the cards inside those stacks. When deleting a stack, all cards and sessions linked to that stack will also be deleted
 
* Exit closes the program
