using Flashcards._0lcm.CRUDController;
using Flashcards._0lcm.DTOs;
using Flashcards._0lcm.Interfaces;
using Flashcards._0lcm.Models;

namespace Flashcards._0lcm.Services;

public class StackService : IStackService
{
    public void CreateStack(string? name)
    {
        if (TryValidateStackName(name)) LocalDbController.CreateStack(new Stack { Name = name! });
    }

    public void UpdateStack(StackDto dto, string? name)
    {
        if (TryValidateStackName(name))
        {
            var stack = new Stack
            {
                StackId = dto.StackId,
                Name = name!
            };
            LocalDbController.UpdateStack(stack);
        }
    }

    public void DeleteStack(StackDto dto)
    {
        var stack = new Stack
        {
            StackId = dto.StackId,
            Name = dto.Name
        };
        LocalDbController.DeleteStackAndDependents(stack);
    }

    public List<StackDto> GetStackDtos()
    {
        var stacks = LocalDbController.GetStacks()
            .OrderBy(s => s.Name);

        var stackDtos = new List<StackDto>();

        var displayId = 1;
        foreach (var stack in stacks)
            stackDtos.Add(new StackDto
            {
                StackId = stack.StackId,
                DisplayId = displayId++,
                Name = stack.Name,
                FlashcardCount = LocalDbController.GetFlashcardCountForStack(stack.StackId)
            });

        return stackDtos;
    }

    private static bool TryValidateStackName(string? name)
    {
        if (string.IsNullOrEmpty(name)) throw new ArgumentNullException("Name is null or empty.");

        var stacks = LocalDbController.GetStacks();
        foreach (var stack in stacks)
            if (name == stack.Name)
                throw new ArgumentException("Name is already taken.");

        return true;
    }
}