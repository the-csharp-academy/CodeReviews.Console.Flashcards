using Flashcards.DTOs;
using Flashcards.Entities;
using Flashcards.Repositories;
using Microsoft.Extensions.Configuration;

namespace Flashcards.Controllers
{
    public class CardStackController
    {
        private readonly CardStackRepository repository;

        public CardStackController()
        {
            var configuration = new ConfigurationBuilder()
                .SetBasePath(AppContext.BaseDirectory)
                .AddJsonFile("appsettings.json", optional: false)
                .Build();

            var connectionString = configuration.GetConnectionString("DefaultConnection");
            repository = new(connectionString);
            repository.CreateTable();
        }

        public bool CreateCardStack(CardStackDTO cardStack)
        {
            var cardEntity = new CardStackEntity
            {
                Name = cardStack.Name,
            };
            return repository.CreateCardStack(cardEntity);
        }

        public IEnumerable<CardStackDTO> ReadCardStacks()
        {
            var stacks = repository.ReadCardStacks();
            foreach (var cardStack in stacks)
                yield return new CardStackDTO { Id = cardStack.Id, Name = cardStack.Name };
        }

        public void DeleteCardStack(int stackId)
        {
            repository.DeleteCardStack(stackId);
        }

    }
}
