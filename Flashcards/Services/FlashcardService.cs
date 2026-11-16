using Flashcards.DTOs;
using Flashcards.Entities;
using Flashcards.Interfaces;
using Flashcards.Validation;

namespace Flashcards.Services
{
    public class FlashcardService
    {
        private readonly IFlashcardRepository _flashcardRepository;

        public FlashcardService(IFlashcardRepository flashcardRepository)
        {
            _flashcardRepository = flashcardRepository;
        }
        public async Task<List<FlashcardDTO>> GetAllFlashcardsAsync(int? stackId)
        {
            var flashcards = await _flashcardRepository.GetAllFlashcardsAsync(stackId);

            return await CreateFlashcardDTOs(flashcards);
        }

        public async Task<List<FlashcardDTO>> CreateFlashcardDTOs(List<Flashcard> flashcards)
        {

            var displayCards = new List<FlashcardDTO>();

            foreach (var flashcard in flashcards)
            {
                bool validFlashcard = await FlashcardValidator.ValidateFlashcard(flashcard);

                if (!validFlashcard)
                {
                    continue;
                }

                var displayCard = new FlashcardDTO
                {
                    Id = flashcard.Id,
                    Number = displayCards.Count + 1,
                    Front = flashcard.Front,
                    Back = flashcard.Back,
                    StackId = flashcard.StackID
                };

                displayCards.Add(displayCard);
            }

            return displayCards;
        }

        public async Task UpdateFlashcardAsync(FlashcardDTO flashcardDTO)
        {
            await _flashcardRepository.UpdateFlashcardAsync(flashcardDTO);
        }

        public async Task CreateFlashcardAsync(FlashcardDTO flashcard)
        {
            await _flashcardRepository.CreateFlashcardAsync(flashcard);
        }

        public async Task DeleteFlashcardAsync(FlashcardDTO flashcard)
        {
            await _flashcardRepository.DeleteFlashcardAsync(flashcard);
        }

        public async Task<FlashcardDTO> CreateFlashcardDTOFromInput(StackDTO stack, string front, string back)
        {
            var flashcardDTO = new FlashcardDTO
            {
                StackId = stack.Id,
                Front = front,
                Back = back
            };

            return flashcardDTO;
        }

        public async Task<List<FlashcardDTO>> GetFlashcardsByStackAsync(StackDTO stack)
        {
            return await GetAllFlashcardsAsync(stack.Id);
        }
    }
}
