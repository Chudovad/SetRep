using Microsoft.AspNetCore.Components.WebAssembly.Hosting;
using Microsoft.Extensions.Configuration;
using MudBlazor.Services;
using SetRep.WebApp.Client.Services;

var builder = WebAssemblyHostBuilder.CreateDefault(args);

builder.Services.AddMudServices();

// Настройка HttpClient для API
builder.Services.AddScoped(sp =>
{
    var configuration = sp.GetRequiredService<IConfiguration>();
    var apiBaseUrl = configuration["ApiBaseUrl"] ?? "https://localhost:7097";
    var httpClient = new HttpClient
    {
        BaseAddress = new Uri(apiBaseUrl)
    };
    return httpClient;
});

// Регистрация сервиса Telegram WebApp
builder.Services.AddScoped<ITelegramWebAppService, TelegramWebAppService>();

await builder.Build().RunAsync();
