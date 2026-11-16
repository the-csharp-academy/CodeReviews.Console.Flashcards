using Flashcards.DTOs;
using Flashcards.Entities;
using Flashcards.Repositories;

namespace Flashcards.Services
{
    public class StackService
    {
        private StackRepository _stackRepository;

        public StackService(StackRepository stackRepository)
        {
            _stackRepository = stackRepository;
        }
        public async Task<List<StackDTO>> GetAllStacksDTOAsync(List<Stack>? stackList)
        {
            var stacks = new List<Stack>();

            if (stackList == null)
            {
                stacks = await _stackRepository.GetAllStacksAsync();
            }
            else
            {
                stacks = stackList;
            }

            return stacks.Select((s, index) => new StackDTO
            {
                Id = s.Id,
                Number = index + 1,
                Name = s.Name
            }).ToList();
        }

        public async Task<List<StackDTO>> GetAllStacksWithFlashcardsAsync()
        {
            var stackList = await _stackRepository.GetStacksWithAvailableFlashcardsAsync();

            return await GetAllStacksDTOAsync(stackList);
        }

        public async Task CreateStackAsync(string name)
        {
            await _stackRepository.CreateStackAsync(name);
        }

        public async Task UpdateStackAsync(StackDTO stack, string name)
        {
            await _stackRepository.UpdateStackAsync(stack, name);
        }

        public async Task DeleteStackAsync(StackDTO stack)
        {
            await _stackRepository.DeleteStackAsync(stack);
        }

        public async Task<Stack?> GetStackByIdAsync(int stackId)
        {
            return await _stackRepository.GetStackByIdAsync(stackId);
        }
    }
}
