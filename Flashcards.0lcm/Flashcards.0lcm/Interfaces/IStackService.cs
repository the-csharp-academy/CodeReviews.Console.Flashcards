using Flashcards._0lcm.DTOs;

namespace Flashcards._0lcm.Interfaces;

public interface IStackService
{
    void CreateStack(string? name);
    void UpdateStack(StackDto dto, string? name);
    void DeleteStack(StackDto dto);

    List<StackDto> GetStackDtos();
}