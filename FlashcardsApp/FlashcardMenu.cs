using System;
using FlashcardsApp.DTOs;
using FlashcardsApp.Models;
using FlashcardsApp.Repositories;
using FlashcardsApp.Utilities;

namespace FlashcardsApp
{
    public class FlashcardMenu
    {
        private readonly FlashcardRepository _flashcardRepo;
        private readonly SessionRepository _sessionRepo;
        private readonly FStack _fs;

        private const int Width = 50;

        public FlashcardMenu(FlashcardRepository flashcardRepo, FStack fs, SessionRepository  sessionRepo)
        {
            _flashcardRepo = flashcardRepo;
            _sessionRepo = sessionRepo;
            _fs = fs;
        }

        public void ShowFlashcard()
        {
            int c = -1;
            int width = 25;
            string border = "+" + new string('-', 2 * width) + "+";
            string line = new string('-', width * 2);

            while (c != 0)
            {   
                FStackDTO fStackDTO = new FStackDTO { Name = _fs.Name };
                List<Flashcard> ls = _flashcardRepo.GetAllOfStack(_fs);
                List<FlashcardDTO> list = new List<FlashcardDTO>();
                foreach (var f in ls)
                {
                    list.Add(new FlashcardDTO { Front = f.Front, Back = f.Back });
                }

                Console.Clear();
                Console.WriteLine($"\n" + " {fStackDTO.Name} ".PadCenter(Width, '='));
                Console.WriteLine(border);
                Console.WriteLine($"| ID | {"FRONT".PadRight(width - 5)} | {"BACK".PadRight(width - 5)} |");
                Console.WriteLine(border);

                for (int i = 0; i < list.Count; i++)
                {
                    Console.WriteLine($"| {(i + 1).ToString().PadRight(1)}  | {list[i].Front.PadRight(width - 5)} | {list[i].Back.PadRight(width - 5)} |");
                    Console.WriteLine(border);
                }

                Console.WriteLine("Type a number to choose a flashcard.");
                Console.WriteLine("Type 'A' to create new flashcard in this stack.");
                Console.WriteLine("Type 'S' to start studying.");
                Console.WriteLine("Type 0 to exit.");
                Console.Write("Your choice:\t");
                string? inp = Console.ReadLine();

                if (inp != null && (inp.Trim() == "A" || inp.Trim() == "a"))
                {
                    CreateNewFlashcard();
                    return;
                }

                if (inp != null && (inp.Trim() == "S" || inp.Trim() == "s"))
                {
                    Study(list);
                    return;
                }

                while (!int.TryParse(inp, out c) || c < 0 || c > list.Count)
                {
                    Console.Write("\nPlease enter a valid number.\nTry again:\t");
                    inp = Console.ReadLine();
                }

                if (c != 0)
                {
                    FlashcardActionMenu(ls[c - 1]);
                    return;
                }

                else
                {
                    return;
                }
            }
        }

        public void CreateNewFlashcard()
        {
            string? inp;
            string front = "", back = "";

            Console.Clear();
            Console.WriteLine("\n" + " CREATE NEW FLASHCARD ".PadCenter(Width + 2, '='));

            Console.Write("\nFlashcard's front:\t");
            inp = Console.ReadLine();
            while (inp == null)
            {
                Console.Write("\nPlease enter a front.\nTry again:\t");
                inp = Console.ReadLine();
            }
            front = inp;

            Console.Write("\nFlashcard's back:\t");
            inp = Console.ReadLine();
            while (inp == null)
            {
                Console.Write("\nPlease enter a back.\nTry again:\t");
                inp = Console.ReadLine();
            }
            back = inp;

            var newFlashcard = new FlashcardDTO { Front = front, Back = back };

            try
            {
                bool isSuccess = _flashcardRepo.Insert(newFlashcard, _fs);

                if (isSuccess)
                {
                    Console.WriteLine("\nCreated new stack successfully!");
                }
                else
                {
                    Console.WriteLine("\nError! Please try again!");
                }
            }

            catch (Exception e)
            {
                Console.WriteLine($"\nError occured: {e.Message}");
            }

            finally
            {
                Console.WriteLine("\nPress ENTER to continue: ");
                Console.ReadLine();
            }
        }
    
        public void FlashcardActionMenu(Flashcard flashcard)
        {
            int c;
            int width = 25;
            string border = "+" + new string('-', 2 * width) + "+";
            string line = new string('-', width * 2);
            string? inp;

            Console.Clear();
            Console.WriteLine("\n" + $" Current working flashcard ".PadCenter(Width, '='));
            Console.WriteLine(border);
            Console.WriteLine($"| {flashcard.Front.PadRight(width - 3)} | {flashcard.Back.PadRight(width - 2)} |");
            Console.WriteLine(border);
            Console.WriteLine("| 1 | Edit flashcard.".PadRight(Width) + " |");
            Console.WriteLine("| 2 | Delete flashcard.".PadRight(Width) + " |");
            Console.WriteLine("| 0 | Exit.".PadRight(Width) + " |");
            Console.WriteLine(border);

            inp = Console.ReadLine();
            while (!int.TryParse(inp, out c) || c < 0 || c > 2)
            {
                Console.Write("\nPlease enter a valid number.\nTry again: ");
                inp = Console.ReadLine();
            }

            switch (c)
            {
                case 0:
                    return;
                case 1:
                    EditFlashcard(flashcard);
                    return;
                case 2:
                    DeleteFlashcard(flashcard);
                    return;
                default:
                    return;
            }
        }

        public void EditFlashcard(Flashcard flashcard)
        {
            int id = flashcard.Id, stackId = flashcard.StackId;
            string front = flashcard.Front;
            string back = flashcard.Back;
            string? inp;

            Console.WriteLine($"\nCurrent flashcard front: {flashcard.Front}");
            Console.Write("\nPlease enter new front for this flashcard:\t");
            inp = Console.ReadLine();
            if (inp == null)
            {
                return;
            }
            front = inp;

            Console.WriteLine($"\nCurrent flashcard back: {flashcard.Back}");
            Console.Write("\nPlease enter new back for this flashcard:\t");
            inp = Console.ReadLine();
            if (inp == null)
            {
                return;
            }
            back = inp;

            var newFlashcard = new Flashcard { Id = id, StackId = stackId, Front = front, Back = back };

            try
            {
                bool isSuccess = _flashcardRepo.Update(newFlashcard);

                if (isSuccess)
                {
                    Console.WriteLine("\nUpdated flashcard successfully!");
                }
                else
                {
                    Console.WriteLine("\nError! Please try again!");
                }
            }

            catch (Exception e)
            {
                Console.WriteLine($"Error occured: {e.Message}");
            }

            finally
            {
                Console.WriteLine("\nPress ENTER to continue: ");
                Console.ReadLine();
            }
        }

        public void DeleteFlashcard(Flashcard flashcard)
        {
            try
            {
                bool isSuccess = _flashcardRepo.Delete(flashcard);

                if (isSuccess)
                {
                    Console.WriteLine("\nSuccessfully delete flashcard!");
                }
                else
                {
                    Console.WriteLine("\nError! Please try again!");
                }
            }
            catch (Exception e)
            {
                Console.WriteLine($"\nError occured: {e.Message}");
            }
            finally
            {
                Console.WriteLine("\nPress ENTER to continue: ");
                Console.ReadLine();
            }
        }

        public void Study(List<FlashcardDTO> list)
        {
            int width = 25;
            int score = 0;
            string border = "+" + new string('-', width) + "+";
            string line = new string('-', width);
            string? inp;

            Console.Clear();
            foreach (var flashcard in list) 
            {
                Console.WriteLine("\n"+border);
                Console.WriteLine($"| {"FRONT".PadRight(width)} |");
                Console.WriteLine(border);
                Console.WriteLine($"| {flashcard.Front.PadRight(width)} |");
                Console.WriteLine(border);
                Console.WriteLine("Enter your answer for this or type 0 to Exit.");
                Console.Write("Your answer:\t");
                inp = Console.ReadLine();

                if(inp == null || inp.Equals("0"))
                {
                    return;
                }

                if (inp.ToLower().Equals(flashcard.Back.ToLower()))
                {
                    score += 1;
                    Console.WriteLine("Correct!");
                }

                else
                {
                    Console.WriteLine($"\nWrong answer!\nCorrect Answer: {flashcard.Back}.");
                }
            }
            Console.WriteLine(line);
            Console.WriteLine($"Your final score: {score}/{list.Count}");
            Console.WriteLine(line);

            SessionDTO ss = new SessionDTO{Date =  DateTime.Now, Score = score};
            CreateSession(ss);

            Console.WriteLine("\nPress ENTER to continue: ");
            Console.ReadLine();
        }

        public void CreateSession(SessionDTO ss)
        {
            try
            {
                bool isSuccess = _sessionRepo.Insert(ss, _fs);

                if (isSuccess)
                {
                    Console.WriteLine("\nStudy session saved successfully!");
                }
                else
                {
                    Console.WriteLine("\nError occured while trying to save study session!");
                }
            }

            catch (Exception e)
            {
                Console.WriteLine("\nError occured while trying to save study session!");
            }
        }
    }
}
