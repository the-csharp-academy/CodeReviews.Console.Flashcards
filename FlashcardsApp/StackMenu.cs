using System;
using FlashcardsApp.DTOs;
using FlashcardsApp.Models;
using FlashcardsApp.Repositories;
using FlashcardsApp.Utilities;

namespace FlashcardsApp
{
    public class StackMenu
    {
        private readonly StackRepository _stackRepo;
        private readonly FlashcardRepository _flashcardRepo;
        private readonly SessionRepository _sessionRepo;

        private const int Width = 50;
        private readonly string border = "+" + new string('-', Width) + "+";
        private readonly string line = new string('-', Width+2);

        public StackMenu(StackRepository stackRepo, FlashcardRepository flashcardRepo, SessionRepository sessionRepo)
        {
            _stackRepo = stackRepo;
            _flashcardRepo = flashcardRepo;
            _sessionRepo = sessionRepo;
        }

        public void ShowStackMenu()
        {
            int c = -1;

            while (c != 0)
            {
                List<FStack> ls = _stackRepo.GetAll();
                List<FStackDTO> list = new List<FStackDTO>();
                foreach(var i in ls)
                {
                    list.Add(new FStackDTO { Name = i.Name });
                }

                Console.Clear();
                Console.WriteLine("\n" + " STACK LIST ".PadCenter(Width + 2, '='));
                Console.WriteLine(border);
                Console.WriteLine($"| ID | {"NAME".PadRight(Width-7)} |");
                Console.WriteLine(border);

                for (int i = 0; i < list.Count; i++)
                {
                    Console.WriteLine($"| {(i+1).ToString().PadRight(1)}  | {list[i].Name.PadRight(Width-7)} |");
                    Console.WriteLine(border);
                }
                Console.WriteLine("Type a number to choose a stack.");
                Console.WriteLine("Type 'A' to create new stack.");
                Console.WriteLine("Type 0 to exit.");
                Console.WriteLine(line);
                Console.Write("Your choice:\t");
                String? inp = Console.ReadLine();

                if(inp != null && (inp.Trim() == "A" || inp.Trim() == "a"))
                {
                    CreateNewStack();
                    continue;
                }

                while (!int.TryParse(inp, out c) || c < 0 || c > list.Count)
                {
                    Console.Write("Please enter a valid number.\nTry again:\t");
                    inp = Console.ReadLine();
                }
                
                if(c != 0)
                {
                    StackActionMenu(ls[c-1]);
                    continue;
                }

                else
                {
                    continue;
                }
            }
        }
        public void CreateNewStack()
        {
            string? inp;
            string name = "";

            Console.Clear();
            Console.WriteLine("\n" + " CREATE NEW STACK ".PadCenter(Width + 2, '='));

            Console.Write("\nStack name:\t");
            inp = Console.ReadLine();
            while(inp == null)
            {
                Console.Write("\nPlease enter a valid name.\nTry again:\t");
                inp = Console.ReadLine();
            }
            name = inp;

            var newStack = new FStackDTO { Name = name };

            try
            {
                bool isSuccess = _stackRepo.Insert(newStack);

                if (isSuccess)
                {
                    Console.WriteLine("\nCreated new stack successfully!");
                    Console.WriteLine($"Stack name: {newStack.Name}.");
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

        public void StackActionMenu(FStack fstack)
        {
            int c;
            string? inp;

            Console.Clear();
            Console.WriteLine(border);
            Console.WriteLine($"| Current working stack: {fstack.Name}.".PadRight(Width) + " |");
            Console.WriteLine(border);
            Console.WriteLine("| 1 | View all flashcards in stack.".PadRight(Width) + " |");
            Console.WriteLine("| 2 | Edit stack's name.".PadRight(Width) + " |");
            Console.WriteLine("| 3 | Delete stack.".PadRight(Width) + " |");
            Console.WriteLine("| 0 | Exit.".PadRight(Width) + " |");
            Console.WriteLine(border);

            inp = Console.ReadLine();
            while (!int.TryParse(inp, out c) || c < 0 || c > 3)
            {
                Console.Write("\nPlease enter a valid number.\nTry again: ");
                inp = Console.ReadLine();
            }

            switch (c)
            {
                case 0:
                    return;
                case 1:
                    showStackFlashcard(fstack);
                    return;
                case 2:
                    editStack(fstack);
                    return;
                case 3:
                    deleteStack(fstack);
                    return;
                default:
                    return;
            }
        }

        public void deleteStack(FStack fstack)
        {
            try
            {
                bool isSuccess = _stackRepo.Delete(fstack);

                if (isSuccess)
                {
                    Console.WriteLine($"\nSuccessfully delete stack: {fstack.Name}");
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

        public void editStack(FStack fstack)
        {
            int id = fstack.Id;
            string name = fstack.Name;
            string? inp;

            Console.WriteLine($"Current stack: {name}");
            Console.Write("\nPlease enter new name for this stack:\t");
            inp = Console.ReadLine();
            if (inp == null)
            {
                return;
            }

            name = inp;
            var newStack = new FStack { Id = id, Name = name };

            try
            {
                bool isSuccess = _stackRepo.Update(newStack);

                if (isSuccess)
                {
                    Console.WriteLine("\nUpdated stack successfully!");
                    Console.WriteLine($"Stack name: {newStack.Name}.");
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
        
        public void showStackFlashcard(FStack fstack)
        {
            FlashcardMenu fm = new FlashcardMenu(_flashcardRepo, fstack, _sessionRepo);
            fm.ShowFlashcard();
            return;
        }
    }
}
