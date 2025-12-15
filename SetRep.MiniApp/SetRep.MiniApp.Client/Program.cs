using Microsoft.AspNetCore.Components.WebAssembly.Hosting;
using MudBlazor.Services;
using SetRep.MiniApp.Client.Services;

var builder = WebAssemblyHostBuilder.CreateDefault(args);

builder.Services.AddMudServices();

// Настройка HTTP клиента для API
var apiBaseUrl = builder.Configuration["ApiBaseUrl"] ?? "https://localhost:7097";
builder.Services.AddScoped(sp => new HttpClient 
{ 
    BaseAddress = new Uri(apiBaseUrl) 
});

// Регистрация сервисов для работы с Telegram WebApp
builder.Services.AddTransient<ITelegramWebAppService, TelegramWebAppService>();
builder.Services.AddScoped<ITelegramWebAppStateService, TelegramWebAppStateService>();
builder.Services.AddTransient<ITelegramBackButtonService, TelegramBackButtonService>();
builder.Services.AddTransient<IWorkoutHistoryService, WorkoutHistoryService>();
builder.Services.AddScoped<IAppInitializationService, AppInitializationService>();

await builder.Build().RunAsync();
