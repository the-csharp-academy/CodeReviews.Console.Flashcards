using Kerem.Flashcards.Domain.Entities;
using Kerem.Flashcards.Domain.Interfaces;
using Microsoft.Extensions.Configuration;
using Spectre.Console;

namespace Kerem.Flashcards.Features ;

    public class GetAllStudySessions
    {
        private readonly IStudySessionRepository _repository;

        public GetAllStudySessions(IStudySessionRepository repository)
        {
            _repository = repository;
        }

        public void GetAll()
        {
            List<StudySession> studySessions = _repository.GetAllStudySessions();
            if (!studySessions.Any())
            {
                AnsiConsole.MarkupLine("[red]No sessions found[/]");
                return;
            }
            var table = new Table();
            
            table.AddColumn("Date");
            table.AddColumn("Score");
            
            foreach (var session in studySessions)
            {
                table.AddRow(session.CreatedAt.ToString(), session.Score.ToString());
            }
            AnsiConsole.Write(table);
        }
    }