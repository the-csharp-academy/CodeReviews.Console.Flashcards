using Flashcards.Interfaces;
using Flashcards.Repositories;
using Flashcards.Services;
using Flashcards.UI;
using Microsoft.Extensions.DependencyInjection;

namespace Flashcards.Extensions
{
    public static class ServiceCollectionExtension
    {
        public static IServiceCollection AddCustomServices(this IServiceCollection services)
        {
            services.AddScoped<StackRepository>();
            services.AddScoped<IFlashcardRepository, FlashcardRepository>();
            services.AddTransient<MainMenu>();
            services.AddTransient<StackMenu>();
            services.AddTransient<FlashcardMenu>();
            services.AddScoped<FlashcardService>();
            services.AddScoped<StackService>();
            services.AddTransient<StudySessionMenu>();
            services.AddScoped<StudySessionRepository>();
            services.AddScoped<StudySessionService>();
            services.AddTransient<StudySessionUI>();
            services.AddScoped<ReportingRepository>();
            services.AddScoped<ReportingService>();
            services.AddTransient<ReportsMenu>();

            return services;
        }
    }
}
