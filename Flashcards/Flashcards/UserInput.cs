public class Input
{
    //handles the menu and user input
    private static int stackId;

    public static void GetUserInput()
    {
        bool end = false;
        while (end == false)
        {
            Console.WriteLine("Welcome to the Flashcard Revision system!\n");
            Console.WriteLine("Please choose an option!\n");
            Console.WriteLine("Press 0 to close the console.");
            Console.WriteLine("Press 1 to Manage Revision Stacks.");
            Console.WriteLine("Press 2 to Manage Flashcards.");
            Console.WriteLine("Press 3 to go to the study sessions area.");
            Console.WriteLine("Press 4 to view previous Study Sessions.");


            string command = Console.ReadLine();


            switch (command)
            {
                case "0":
                    Console.WriteLine("Goodbye\nPress Any Key to exit.");
                    Console.ReadLine();
                    Environment.Exit(0);
                    break;


                case "1":
                    ManageStacks();
                    break;

                case "2":
                    ManageFlashcards();
                    break;

                case "3":
                    ManageSession();
                    break;

                case "4":
                    DatabaseController.PreviousSessions();
                    break;

            }
        }
    }

    public static void ManageStacks()
    {

        bool end = false;
        while (end == false)
        {
            Console.Clear();
            Console.WriteLine("Welcome to the Stack system!\n");
            Console.WriteLine("What would you like to do?");
            Console.WriteLine("Press 0 to return to the main menu.");
            Console.WriteLine("Press 1 to view all Revision Stacks.");
            Console.WriteLine("Press 2 to Insert a Revision Stack.");
            Console.WriteLine("Press 3 to Delete a Revision Stack.");
            Console.WriteLine("Press 4 to Update a Revision Stack name.");


            string command = Console.ReadLine();

            switch (command)
            {
                case "0":
                    Console.WriteLine("Goodbye\nPress Any Key to exit.");
                    Console.ReadLine();
                    GetUserInput();
                    break;


                case "1":
                    DatabaseController.ViewStackRecords();
                    break;

                case "2":
                    InsertStackInput();
                    break;

                case "3":
                    DeleteStackInput();
                    break;

                case "4":
                    UpdateStackInput();
                    break;

                default:
                    Console.WriteLine("Please enter e value between 1 & 4");
                    break;

            }
        }
    }


    public static void InsertStackInput()
    {
        Console.WriteLine("Please enter the name of your revision stack.");
        Console.WriteLine("\nEnter 0 if you would like to return to main menu.");

        string stackInput = Console.ReadLine();

        if (stackInput == "0") GetUserInput();

        DatabaseController.InsertStack(stackInput);
    }

    public static void DeleteStackInput()
    {
        Console.Clear();
        DatabaseController.ViewStackRecords();

        Console.WriteLine("Please enter the name of the revision stack you would like to delete.");
        Console.WriteLine("\nEnter 0 if you would like to return to main menu.");
        string name = Console.ReadLine();

        if (name == "0") return;

        DatabaseController.DeleteStackByName(name);
        //insert delinput into the delete method.

    }

    public static void UpdateStackInput()
    {
        Console.Clear();
        DatabaseController.ViewStackRecords();
        Console.WriteLine("Please enter the name of the stack you would like to update.");
        Console.WriteLine("\nEnter 0 if you would like to return to main menu.");

        string OldName = Console.ReadLine();

        if (OldName == "0") return;

        Console.WriteLine("Please enter the Name you would like the new stack to be.");
        Console.WriteLine("\nEnter 0 if you would like to return to main menu.");

        string NewName = Console.ReadLine();

        if (NewName == "0") return;

        DatabaseController.UpdateStackByName(OldName, NewName);
    }




    public static void ManageFlashcards()
    {

        DatabaseController.ViewStackRecords();
        Console.WriteLine("Enter the ID of the stack to manage: ");
        stackId = Validation.ValidateId(Console.ReadLine());

        bool start = false;
        while (start == false)
        {
            Console.WriteLine();
            Console.WriteLine("Welcome to the Flashcard Menu!\n");
            Console.WriteLine("What would you like to do with the record?");
            Console.WriteLine("Press 0 to return to the main menu.");
            Console.WriteLine("Press 1 to view all Revision Flashcard.");
            Console.WriteLine("Press 2 to Insert a Revision Flashcard.");
            Console.WriteLine("Press 3 to Delete a Revision Flashcard.");
            Console.WriteLine("Press 4 to Update a Revision Flashcard.");

            string command = Console.ReadLine();

            switch (command)
            {
                case "0":
                    Console.WriteLine("Goodbye\nPress Any Key to exit.");
                    Console.ReadLine();
                    GetUserInput();
                    break;
                // Environment.Exit(0);

                case "1":
                    DatabaseController.ViewFlashcardRecords(stackId);
                    break;

                case "2":
                    InsertFlashcardInput();
                    break;

                case "3":
                    DeleteFlashcardInput();
                    break;

                case "4":
                    UpdateFlashcardInput();
                    break;

                default:
                    Console.WriteLine("Please enter a value between 1 & 4");
                    break;

            }

        }
    }

    public static void InsertFlashcardInput()
    {
        Console.Clear();
        Console.WriteLine("Please enter the front and back of your flashcard when prompted.\n");

        Console.WriteLine("Front: ");

        string front = Console.ReadLine();

        Console.WriteLine("Back: ");
        string back = Console.ReadLine();

        DatabaseController.InsertFlashcard(front, back, stackId);
    }

    public static void DeleteFlashcardInput()
    {
        Console.Clear();
        DatabaseController.ViewFlashcardRecords(stackId);

        Console.WriteLine("Please enter the ID of the record you would like to delete.");
        string delInput = Console.ReadLine();

        int finalInput = Validation.ValidateId(delInput);

        DatabaseController.DeleteFlashcard(finalInput, stackId);
    }

    public static void UpdateFlashcardInput()
    {
        Console.Clear();
        DatabaseController.ViewFlashcardRecords(stackId);
        // pass records because the view method needs to know which stacks cards to show.
        Console.WriteLine("Please enter the ID of the record you would like to update.");

        int updateInput = Validation.ValidateId(Console.ReadLine());

        Console.WriteLine("Please enter the front and back of your flashcard to update when prompted.\n");

        Console.WriteLine("Front");
        string front = Console.ReadLine();

        Console.WriteLine("Back");
        string back = Console.ReadLine();

        DatabaseController.UpdateFlashcard(updateInput, stackId, front, back);
    }




    /// <summary>
    /// Session 
    /// </summary>



    public static void StartSession()
    {
        int score = 0;
        // DatabaseController.ViewFlashcardRecords(stackId);

        List<FlashcardDTO> cards = DatabaseController.GetFlashcards(stackId);

        foreach (var card in cards)
        {
            Console.Clear();
            Console.WriteLine($"Front: {card.Front}");
            Console.WriteLine("\nPress any key to see the answer...");
            Console.ReadKey();


            Console.WriteLine($"Back: {card.Back}");
            Console.WriteLine("\nDid you get it right? (Y/N)");

            string answer = Console.ReadLine().ToLower().Trim();
            while (answer != "y" && answer != "n")
            {
                Console.WriteLine("Please enter Y or N.");
                answer = Console.ReadLine().ToLower().Trim();
            }

            if (answer == "y") score++;

        }
        Console.WriteLine($"\nSession complete! You scored {score}");
        Console.ReadKey();

        DatabaseController.InsertSession(stackId, $"{score}");

    }



    public static void ManageSession()
    {

        Console.Clear();
        Console.WriteLine("Welcome to the study sessions area where you will test you knowledge!\nPress any key to continue.");
        Console.ReadKey();
        Console.WriteLine("\nAre you ready to test your knowledge? (Y/N)");
        string answer = Console.ReadLine().ToLower().Trim();

        while (answer != "y" && answer != "n")
        {
            Console.WriteLine("Please enter Y or N.");
            answer = Console.ReadLine().ToLower().Trim();
        }

        if (answer == "y")
        {
            DatabaseController.ViewStackRecords();
            Console.WriteLine("Please enter the ID of the stack you want to study: ");
            stackId = Validation.ValidateId(Console.ReadLine());
            Input.StartSession();
            Console.ReadKey();
            return;
        }

        if (answer == "n")
        {
            Console.WriteLine("Goodbye for now, come back when you're ready!");
            Console.ReadKey();
            return;
        }

        Input.StartSession();
    }

}




