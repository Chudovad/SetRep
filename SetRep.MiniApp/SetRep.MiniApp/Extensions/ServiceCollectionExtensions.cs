using Microsoft.JSInterop;
using MudBlazor.Services;
using SetRep.Core.ApiRequestServices;
using SetRep.MiniApp.Client.Services;

namespace SetRep.MiniApp.Extensions;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddApplicationServices(this IServiceCollection services, WebApplicationBuilder builder)
    {
        // Add MudBlazor services
        services.AddMudServices();

        // Add services to the container.
        services.AddRazorComponents()
            .AddInteractiveServerComponents()
            .AddInteractiveWebAssemblyComponents();

        // Регистрация сервисов для работы с Telegram WebApp (для prerendering)
        // На сервере эти сервисы будут использоваться только при prerendering
        // В WebAssembly они будут переопределены регистрацией из Client/Program.cs
        var apiBaseUrl = builder.Configuration["ApiBaseUrl"] ?? "https://localhost:7097";

        // Регистрация HttpClient для серверных компонентов
        services.AddHttpClient("ApiClient", client =>
        {
            client.BaseAddress = new Uri(apiBaseUrl);
        });

        // Регистрация HttpClient для прямой инъекции (для совместимости с WebAssembly)
        services.AddScoped(sp =>
        {
            var httpClientFactory = sp.GetRequiredService<IHttpClientFactory>();
            return httpClientFactory.CreateClient("ApiClient");
        });

        // Регистрация сервисов для работы с Telegram WebApp
        services.AddTransient<ITelegramWebAppService, TelegramWebAppService>();
        services.AddScoped<ITelegramWebAppStateService, TelegramWebAppStateService>();
        services.AddScoped<ITelegramBackButtonService, TelegramBackButtonService>();

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
