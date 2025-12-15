using Microsoft.JSInterop;
using MudBlazor.Services;
using SetRep.MiniApp.Client.Services;
using SetRep.MiniApp.Components;

var builder = WebApplication.CreateBuilder(args);

// Add MudBlazor services
builder.Services.AddMudServices();

// Add services to the container.
builder.Services.AddRazorComponents()
    .AddInteractiveServerComponents()
    .AddInteractiveWebAssemblyComponents();

// Регистрация сервисов для работы с Telegram WebApp (для prerendering)
// На сервере эти сервисы будут использоваться только при prerendering
// В WebAssembly они будут переопределены регистрацией из Client/Program.cs
var apiBaseUrl = builder.Configuration["ApiBaseUrl"] ?? "https://localhost:7097";

// Регистрация HttpClient для серверных компонентов
builder.Services.AddHttpClient("ApiClient", client =>
{
    client.BaseAddress = new Uri(apiBaseUrl);
});

// Регистрация HttpClient для прямой инъекции (для совместимости с WebAssembly)
builder.Services.AddScoped(sp =>
{
    var httpClientFactory = sp.GetRequiredService<IHttpClientFactory>();
    return httpClientFactory.CreateClient("ApiClient");
});

builder.Services.AddTransient<ITelegramWebAppService>(sp =>
{
    var httpClient = new HttpClient { BaseAddress = new Uri(apiBaseUrl) };
    var jsRuntime = sp.GetService<IJSRuntime>() ?? throw new InvalidOperationException("IJSRuntime is not available");
    var logger = sp.GetRequiredService<ILogger<TelegramWebAppService>>();
    return new TelegramWebAppService(jsRuntime, httpClient, logger);
});
builder.Services.AddScoped<ITelegramWebAppStateService, TelegramWebAppStateService>();
builder.Services.AddScoped<ITelegramBackButtonService>(sp =>
{
    var jsRuntime = sp.GetService<IJSRuntime>() ?? throw new InvalidOperationException("IJSRuntime is not available");
    var logger = sp.GetRequiredService<ILogger<TelegramBackButtonService>>();
    return new TelegramBackButtonService(jsRuntime, logger);
});
builder.Services.AddTransient<IWorkoutHistoryService>(sp =>
{
    var httpClient = sp.GetRequiredService<HttpClient>();
    return new WorkoutHistoryService(httpClient);
});
builder.Services.AddScoped<IAppInitializationService, AppInitializationService>();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseWebAssemblyDebugging();
}
else
{
    app.UseExceptionHandler("/Error", createScopeForErrors: true);
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

// Используем HTTPS редирект только если настроен HTTPS порт
var httpsPort = app.Configuration["ASPNETCORE_HTTPS_PORTS"];
if (!string.IsNullOrEmpty(httpsPort))
{
    app.UseHttpsRedirection();
}


app.UseAntiforgery();

app.MapStaticAssets();
app.MapRazorComponents<App>()
    .AddInteractiveServerRenderMode()
    .AddInteractiveWebAssemblyRenderMode()
    .AddAdditionalAssemblies(typeof(SetRep.MiniApp.Client._Imports).Assembly);

app.Run();
