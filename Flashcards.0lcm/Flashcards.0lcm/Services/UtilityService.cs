using Flashcards._0lcm.DTOs;
using Flashcards._0lcm.Interfaces;
using Flashcards._0lcm.UserInterface;
using Spectre.Console;
using Spectre.Console.Rendering;

namespace Flashcards._0lcm.Services;

internal class UtilityService
{
    internal static Dictionary<string, StackDto?> BuildStackMap(
        IStackService stackService,
        string? returnOption = null,
        string? createOption = null)
    {
        var stackDtos = stackService.GetStackDtos();
        var stackMap = new Dictionary<string, StackDto?>();

        if (returnOption != null) stackMap[returnOption] = null;
        if (createOption != null) stackMap[createOption] = null;

        foreach (var stackDto in stackDtos)
        {
            var display = $"-{stackDto.Name}-";
            stackMap[display] = stackDto;
        }

        return stackMap;
    }

    internal static Dictionary<string, FlashcardDto?> BuildFlashcardMap(
        IFlashcardService flashcardService,
        StackDto stackDto,
        string? returnOption = null, string? createOption = null)
    {
        var flashcards = flashcardService.GetFlashcardDtosForStack(stackDto);
        var flashcardMap = new Dictionary<string, FlashcardDto?>();

        if (returnOption != null) flashcardMap[returnOption] = null;
        if (createOption != null) flashcardMap[createOption] = null;

        foreach (var flashcard in flashcards)
        {
            var display = $"-ID: {flashcard.DisplayId} | {flashcard.Name} : {flashcard.Value}-";
            flashcardMap[display] = flashcard;
        }

        return flashcardMap;
    }

    internal static List<IRenderable> BuildSessionRenderableRows(IStudyService studyService, StackDto dto)
    {
        var sessions = studyService.GetStudySessionDtosForStack(dto);
        var list = new List<IRenderable>();

        foreach (var session in sessions)
            list.Add(new Markup(
                $"[{DisplayHelper.White}]ID: {session.DisplayId} | Flashcards studied: {session.StudyCount}[/]"));

        return list;
    }
}