using Flashcards.Controllers;
using Flashcards.DTOs;

namespace Flashcards.Menus
{
    public class CardStackMenu
    {
        private readonly CardStackController cardStackController;
        private readonly FlashcardMenu flashcardMenu;

        public CardStackMenu(CardStackController cardStackController, FlashcardMenu flashcardMenu)
        {
            this.cardStackController = cardStackController;
            this.flashcardMenu = flashcardMenu;
        }

        public void Run()
        {
            while (true)
            {
                Console.Clear();
                var data = cardStackController.ReadCardStacks().ToList();
                UI.PrintStackTable(data);

                var stackAction = UI.GetActions(new List<string>
                {
                    "Add stack",
                    "Remove stack",
                    "Manage flashcards",
                    "Back"
                });

                if (stackAction == "Add stack")
                    AddStack();

                if (stackAction == "Remove stack")
                    RemoveStacks(data);

                if (stackAction == "Manage flashcards")
                    ManageFlashcards(data);

                if (stackAction == "Back")
                    return;
            }
        }

        private void AddStack()
        {
            while (true)
            {
                var name = UI.AskText("Enter stack name:", 50);
                var card = new CardStackDTO { Name = name };
                if (cardStackController.CreateCardStack(card))
                    return;

                UI.PrintMessage("A stack with this name already exists. Enter a different name.");
            }
        }

        private void RemoveStacks(List<CardStackDTO> data)
        {
            var stacks = UI.PrintStackChoices(data);
            if (stacks == null)
                return;

            foreach (var item in stacks)
            {
                cardStackController.DeleteCardStack(item);
            }
        }

        private void ManageFlashcards(List<CardStackDTO> data)
        {
            if (data.Count == 0)
                return;

            var list = data.Select(c => c.Name).ToList();
            var stackName = UI.GetActions(list);
            var stackOwner = data.First(c => c.Name == stackName);
            flashcardMenu.Run(stackOwner);
        }
    }
}
