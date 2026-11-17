using Kerem.Flashcards.Domain.Interfaces;
using Kerem.Flashcards.Features;
using Kerem.Flashcards.Infrastructure.Persistence;
using Kerem.Flashcards.Infrastructure.Repositories;
using Kerem.Flashcards.UI;
using Microsoft.Extensions.DependencyInjection;

namespace Kerem.Flashcards;

    public static class DependencyInjection
    {
        public static IServiceCollection AddApplication(this IServiceCollection services, string connectionString)                                                                                                            
        {               
            services.AddSingleton(new DapperDbContext(connectionString));
            services.AddSingleton<IStackRepository, StackRepository>();
            services.AddSingleton<CreateStack>();
            services.AddSingleton<GetStackByName>();
            services.AddSingleton<ConsoleMenu>();
            return services;
        }   
    }