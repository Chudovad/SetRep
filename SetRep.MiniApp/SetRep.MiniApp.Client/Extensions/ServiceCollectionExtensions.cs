using Microsoft.AspNetCore.Components.WebAssembly.Hosting;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Http;
using MudBlazor.Services;
using SetRep.Core.ApiRequestServices;
using SetRep.MiniApp.Client.Services;

namespace SetRep.MiniApp.Client.Extensions;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddApplicationServices(this IServiceCollection services, WebAssemblyHostBuilder builder)
    {
        // Регистрация MudBlazor сервисов
        services.AddMudServices();

        // Регистрируем TelegramWebAppStateService первым
        services.AddScoped<ITelegramWebAppStateService, TelegramWebAppStateService>();
        
        // Настройка HTTP клиента для API
        var apiBaseUrl = builder.Configuration["ApiBaseUrl"] ?? "https://localhost:7097";
        services.AddScoped(sp => new HttpClient 
        {
            BaseAddress = new Uri(apiBaseUrl) 
        });

        // Регистрация сервисов для работы с Telegram WebApp
        services.AddTransient<ITelegramWebAppService, TelegramWebAppService>();
        services.AddScoped<ITelegramBackButtonService, TelegramBackButtonService>();
        
        // Регистрация бизнес-сервисов
        services.AddScoped<IActiveWorkoutService, ActiveWorkoutService>();
        services.AddTransient<IWorkoutHistoryService, WorkoutHistoryService>();
        services.AddTransient<IWorkoutExerciseService, WorkoutExerciseService>();
        services.AddTransient<IExerciseService, ExerciseService>();
        services.AddTransient<IWorkoutService, WorkoutService>();
        services.AddTransient<IWorkoutPlanService, WorkoutPlanService>();
        services.AddTransient<IWorkoutHistorySetService, WorkoutHistorySetService>();
        services.AddTransient<ISetService, SetService>();
        services.AddTransient<IUserService, UserService>();
        services.AddScoped<IAppInitializationService, AppInitializationService>();

        return services;
    }
}
