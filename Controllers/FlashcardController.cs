using Flashcards.DTOs;
using Flashcards.Entities;
using Flashcards.Repositories;
using Microsoft.Extensions.Configuration;

namespace Flashcards.Controllers
{
    public class FlashcardController
    {
        private readonly FlashcardRepository repository;

        public FlashcardController()
        {
            var configuration = new ConfigurationBuilder()
                .SetBasePath(AppContext.BaseDirectory)
                .AddJsonFile("appsettings.json", optional: false)
                .Build();

            var connectionString = configuration.GetConnectionString("DefaultConnection");
            repository = new(connectionString);
            repository.CreateTable();
        }

        public void CreateFlashCard(CreateFlashcardDTO request)
        {
            var entity = new FlashcardEntity
            {
                Front = request.Front,
                Back = request.Back,
                CardStackId = request.CardStackId,
            };
            repository.CreateFlashcard(entity);
        }

        public GetFlashcardDTO? GetFlashcard(int id)
        {
            var entity = repository.GetFlashcard(id);
            if (entity == null)
            {
                return null;
            }
            return new GetFlashcardDTO
            {
                Id = entity.Id,
                Front = entity.Front,
                Back = entity.Back,
            };
        }

        public IEnumerable<GetFlashcardDTO> GetFlashcards(int stackId)
        {
            var entities = repository.ReadFlashcards(stackId);
            foreach (var item in entities)
                yield return new GetFlashcardDTO { Id = item.Id, Front = item.Front, Back = item.Back };
        }

        public void DeleteFlashcard(int id)
        {
            repository.DeleteFlashcard(id);
        }
    }
}
